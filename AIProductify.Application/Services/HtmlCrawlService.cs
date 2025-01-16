using AIProductify.Application.Interfaces;
using AIProductify.Core.Entities;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;


using Attribute = AIProductify.Core.Entities.ProductAttribute;

namespace AIProductify.Application.Services
{
    public class HtmlCrawlService : IHtmlCrawlService
    {
        private readonly HttpClient _httpClient;

        public HtmlCrawlService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<Product> GetProductDataAsync(string productUrl)
        {
            var response = await _httpClient.GetAsync(productUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch product page: {response.StatusCode}");
            }

            var htmlContent = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var extractProduct = ExtractProductDetails(htmlDoc);

            var product = new Product
            {
                Name = ExtractProductName(htmlDoc),
                Description = ExtractProductDescription(htmlDoc),
                Sku = ExtractSku(productUrl),
                ParentSku = ExtractParentSku(htmlDoc),
                Attributes = ExtractAttributes(htmlDoc),
                Category = ExtractCategory(htmlDoc),
                Brand = ExtractBrand(htmlDoc),
                OriginalPrice = extractProduct?["variants"]?[0]?["price"]?["originalPrice"]?["value"]?.ToObject<decimal>() ?? 0,
                DiscountedPrice = extractProduct?["variants"]?[0]?["price"]?["discountedPrice"]?["value"]?.ToObject<decimal>() ?? 0,
                Images = extractProduct?["images"]?.Select(image => image.ToString()).ToList() ?? new List<string>()

            };

            return product;
        }


        private string ExtractProductName(HtmlDocument htmlDoc)
        {
            var nameNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[@class='pr-new-br']");
            return nameNode?.InnerText.Trim() ?? "Can not Find the Product Name";
        }

        private string ExtractProductDescription(HtmlDocument htmlDoc)
        {
            var jsonNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");

            if (jsonNode == null)
            {
                return "Description is Null";
            }

            try
            {
                var jsonContent = jsonNode.InnerText;
                var jsonObject = System.Text.Json.JsonDocument.Parse(jsonContent);

                if (jsonObject.RootElement.TryGetProperty("description", out var descriptionElement))
                {
                    return descriptionElement.GetString()?.Trim() ?? "Description could not find.";
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return "An Error occured";
            }

            return "Description could not find.";
        }

        private string ExtractSku(string productUrl)
        {
            // Fetching sku from Url
            var match = System.Text.RegularExpressions.Regex.Match(productUrl, @"-p-(\d+)");
            return match.Success ? match.Groups[1].Value : "Couldnt find";
        }

        private List<string> ExtractParentSku(HtmlDocument htmlDoc)
        {
            var jsonNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
            if (jsonNode == null)
            {
                return new List<string> { "Could not find parent sku." };
            }

            try
            {
                var jsonContent = jsonNode.InnerText;
                var jsonObject = System.Text.Json.JsonDocument.Parse(jsonContent);

                var parentSkus = new List<string>();

                if (jsonObject.RootElement.TryGetProperty("hasVariant", out var hasVariantArray) &&
                    hasVariantArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var variant in hasVariantArray.EnumerateArray())
                    {
                        if (variant.TryGetProperty("isVariantOf", out var isVariantOfElement) &&
                            isVariantOfElement.TryGetProperty("@id", out var idElement))
                        {
                            var idUrl = idElement.GetString();
                            var match = System.Text.RegularExpressions.Regex.Match(idUrl, @"-p-(\d+)");
                            if (match.Success)
                            {
                                parentSkus.Add(match.Groups[1].Value); 
                            }
                        }
                    }
                }

                return parentSkus.Any() ? parentSkus : new List<string> { "Parent SKU bulunamadı." };
            }
            catch (System.Text.Json.JsonException)
            {
                return new List<string> { "Parent SKU alınamadı." };
            }
        }

        private List<Attribute> ExtractAttributes(HtmlDocument htmlDoc)
        {

            var jsonNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
            if (jsonNode == null)
            {
                return new List<Attribute>();
            }

            try
            {
                var jsonContent = jsonNode.InnerText;
                var jsonObject = System.Text.Json.JsonDocument.Parse(jsonContent);

                if (jsonObject.RootElement.TryGetProperty("additionalProperty", out var propertiesElement))
                {
                    var attributes = new List<Attribute>();
                    foreach (var property in propertiesElement.EnumerateArray())
                    {
                        var key = property.GetProperty("name").GetString();
                        var value = property.GetProperty("unitText").GetString();

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            attributes.Add(new Attribute
                            {
                                Key = key,
                                Name = value
                            });
                        }
                    }

                    return attributes;
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return new List<Attribute>();
            }

            return new List<Attribute>();
        }

        private string ExtractCategory(HtmlDocument htmlDoc)
        {
            var jsonNodes = htmlDoc.DocumentNode.SelectNodes("//script[@type='application/ld+json']");

            if (jsonNodes == null)
            {
                return "Can not find Category Info.";
            }

            foreach (var jsonNode in jsonNodes)
            {
                try
                {
                    var jsonContent = jsonNode.InnerText;
                    var jsonObject = System.Text.Json.JsonDocument.Parse(jsonContent);

                    // "breadcrumb" kısmını al
                    if (jsonObject.RootElement.TryGetProperty("breadcrumb", out var breadcrumbElement) &&
                        breadcrumbElement.TryGetProperty("itemListElement", out var itemListElement))
                    {
                        var categories = new List<string>();
                        foreach (var item in itemListElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("item", out var itemElement) &&
                                itemElement.TryGetProperty("name", out var nameElement))
                            {
                                var name = nameElement.GetString();
                                if (!string.IsNullOrEmpty(name))
                                {
                                    categories.Add(name);
                                }
                            }
                        }

                        return string.Join(" > ", categories);
                    }
                }
                catch (System.Text.Json.JsonException)
                {
                    continue;
                }
            }

            return "Can not find Category Info.";
        }

        private string ExtractBrand(HtmlDocument htmlDoc)
        {
            var jsonNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
            if (jsonNode == null)
            {
                return "Marka bulunamadı.";
            }

            try
            {
                var jsonContent = jsonNode.InnerText;
                var jsonObject = System.Text.Json.JsonDocument.Parse(jsonContent);

                if (jsonObject.RootElement.TryGetProperty("brand", out var brandElement) &&
                    brandElement.TryGetProperty("name", out var brandName))
                {
                    return brandName.GetString();
                }
            }
            catch (System.Text.Json.JsonException)
            {
                return "Marka bilgisi alınamadı.";
            }

            return "Marka bulunamadı.";
        }

        private static JObject ExtractProductDetails(HtmlDocument htmlDoc)
        {
            var scriptNodes = htmlDoc.DocumentNode.SelectNodes("//script");

            if (scriptNodes == null)
                return null;

            foreach (var scriptNode in scriptNodes)
            {
                var scriptContent = scriptNode.InnerText;

                if (!string.IsNullOrEmpty(scriptContent) && scriptContent.Contains("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__"))
                {
                    try
                    {
                        var jsonStart = scriptContent.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__") + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ =".Length;
                        var jsonEnd = scriptContent.IndexOf(";", jsonStart);
                        var jsonString = scriptContent.Substring(jsonStart, jsonEnd - jsonStart).Trim();

                        var jsonObject = JObject.Parse(jsonString);

                        if (jsonObject.ContainsKey("product"))
                        {
                            var product = (JObject)jsonObject["product"];

                            var variants = product["variants"] as JArray;
                            if (variants != null && variants.Count > 0)
                            {
                                var firstVariant = variants[0];
                                var originalPrice = firstVariant["price"]?["originalPrice"]?["value"]?.ToObject<decimal>() ?? 0;
                                var discountedPrice = firstVariant["price"]?["discountedPrice"]?["value"]?.ToObject<decimal>() ?? 0;
                                var imagesArray = product["images"] as JArray;
                                if (imagesArray != null)
                                {
                                    var images = imagesArray.Select(image => image.ToString()).ToList();
                                }

                            }

                            return product;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"JSON ayrıştırma hatası: {ex.Message}");
                    }
                }
            }

            return null;
        }




    }
}
