﻿using HtmlAgilityPack;
using System.Globalization;
using System.Text;
using System.Web;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        private readonly HttpClient httpClient;
        public HtmlReader()
        {
            this.httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }
        public async Task<Seller> GetTurbokResultsAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller ("Turbok", "https://turbok.by");
            var siteNode = new SellerNodes
            {
                NameNode = "div[2]/div[1]/div[1]",
                PriceNode = "div[2]/div[2]/div",
                PictureNode = "div[1]/a/div/img",
                PictureAttribute = "src",
                CatNumberNode = "div[2]/div[1]/div[2]/p[1]",
                StatusNode = "div[2]/div[1]/p",
                CardUrlNode = "div[1]/a"
            };

            ResponseContent responseContent = new();
            try
            {
                
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/search?gender=&gender=&catlist=0&searchText={search}", cancellationToken);
                
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            card.Name = GetName(siteNode.NameNode, cardNode);
                            card.Price = GetPrice(siteNode.PriceNode, cardNode);
                            card.Picture = GetPicture(siteNode.PictureNode, card.UrlPrefix, cardNode, siteNode.PictureAttribute);
                            card.CatNumber = GetCatNumber(siteNode.CatNumberNode, cardNode);
                            card.Status = GetStatus(siteNode.StatusNode, cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            card.CardUrl = GetCardUrl(siteNode.CardUrlNode, cardNode);
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;

        }
        public async Task<Seller> GetMagnitResultAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Minskmagnit", "https://minskmagnit.by");
            var siteNode = new SellerNodes
            {
                NameNode = "div/div[2]/div[1]/div",
                PriceNode = "div/div[2]/div[2]/div",
                PictureNode = "div/div[1]/a/img",
                PictureAttribute = "src",
                CatNumberNode = "div/div[2]/div[1]/span",
                StatusNode = "div/div[2]/div[2]/div[2]/span[1]",
                CardUrlNode = "div/div[1]/a"
            };

            ResponseContent responseContent = new();

            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/site_search?search_term={search}", cancellationToken);

                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/main/div/article/div/section/ul/li");
                    if (nodes != null)
                    {
                        siteModel.IsAvailable = true;
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            card.Name = GetName(siteNode.NameNode, cardNode);
                            card.Price = GetPrice(siteNode.PriceNode, cardNode);
                            card.Picture = GetPicture(siteNode.PictureNode, card.UrlPrefix, cardNode, siteNode.PictureAttribute);
                            card.CatNumber = GetCatNumber(siteNode.CatNumberNode, cardNode);
                            card.Status = GetStatus(siteNode.StatusNode, cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            card.CardUrl = GetCardUrl(siteNode.CardUrlNode, cardNode);
                            card.Name = card.Name.Replace(card.CatNumber, "");
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }


            return siteModel;
        }
        public async Task<Seller> GetAkvilonResultAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Akvilon", "https://akvilonavto.by");
            var siteNode = new SellerNodes
            {
                NameNode = "div/div[1]/div[2]/div[1]",
                PriceNode = "div/div[1]/div[3]/div/span/span[2]",
                PictureNode = "div/div[1]/div[1]/div[1]/a/img",
                PictureAttribute = "data-src",
                CatNumberNode = "",
                StatusNode = "div/div[2]/div[1]/div[1]/div/div/span/span",
                CardUrlNode = "div/div[1]/div[1]/div[1]/a"
            };

            ResponseContent responseContent = new();
            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/catalog/?q={search}", cancellationToken);
                
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"view-showcase\"]/div");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            string name = GetName(siteNode.NameNode, cardNode);
                            card.Price = GetPrice(siteNode.PriceNode, cardNode);
                            card.Picture = GetPicture(siteNode.PictureNode, card.UrlPrefix, cardNode, siteNode.PictureAttribute);
                            card.CatNumber = Splite(ref name);
                            card.Name = name;
                            card.Status = GetStatus(siteNode.StatusNode, cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            card.CardUrl = GetCardUrl(siteNode.CardUrlNode, cardNode);
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;
        }
        public async Task<Seller> GetBelagroResult(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Belagro", "https://1belagro.by");
            var siteNode = new SellerNodes
            {
                NameNode = "td/a",
                PriceNode = "td[2]/div[2]",
                PictureNode = "td[1]/div/a",
                PictureAttribute = "href",
                CatNumberNode = "",
                StatusNode = "td[1]/div/div/span",
                CardUrlNode = "td[1]/a"
            };

            ResponseContent responseContent = new();
            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/search/?q={search}", cancellationToken);
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"search_catalog\"]/table/tbody/tr");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            string name = GetName(siteNode.NameNode, cardNode);
                            //string pictureAttribute = "href";
                            card.Price = GetPrice(siteNode.PriceNode, cardNode);
                            card.Picture = GetPicture(siteNode.PictureNode, card.UrlPrefix, cardNode, siteNode.PictureAttribute);
                            card.CatNumber = SpliteBelagro(ref name);
                            card.Name = name;
                            card.Status = GetStatus(siteNode.StatusNode, cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            card.CardUrl = GetCardUrl(siteNode.CardUrlNode, cardNode);
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;


        }
        public async Task<Seller> GetMazrezervResult(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Mazrezerv", "https://www.mazrezerv.ru");
            var siteNode = new SellerNodes
            {
                NameNode = "td[3]",
                PriceNode = "td[7]",
                PictureNode = "td[2]/a",
                PictureAttribute = "href",
                CatNumberNode = "td[4]",
                StatusNode = "td[5]",
                CardUrlNode = "td[3]/a"
            };

            ResponseContent responseContent = new();
            string encodedSearch = HttpUtility.UrlEncode(search, Encoding.GetEncoding("windows-1251"));

            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/price/?caption={encodedSearch}&search=full", cancellationToken);
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"print\"]/table/tr");
                    if (nodes != null)
                    {
                        
                        foreach (var cardNode in nodes.Skip(2))
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            card.Name = GetName(siteNode.NameNode, cardNode);
                            card.Price = GetPrice(siteNode.PriceNode, cardNode) * SampleViewModel.Rate;
                            card.Picture = GetPicture(siteNode.PictureNode, card.UrlPrefix, cardNode, siteNode.PictureAttribute);
                            card.CatNumber = GetCatNumber(siteNode.CatNumberNode, cardNode);
                            card.Status = GetStatus(siteNode.StatusNode, cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            if (card.Status == "0")
                            {
                                card.IsAvailable = false;
                                card.Status = "Нет в наличии";
                            }
                            else
                            {
                                card.IsAvailable = true;
                                card.Status = $"В наличии {cardNode.SelectSingleNode("td[5]").InnerText} шт.";
                            }

                            card.CardUrl = GetCardUrl(siteNode.CardUrlNode, cardNode);
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;
        }
        private static string GetName(string nameNode, HtmlNode cardNode)
        {
            string? cardName = String.Empty;
            if (cardNode.SelectSingleNode(nameNode) == null) return cardName = "-----";
            cardName = cardNode.SelectSingleNode(nameNode)?.InnerText.Trim().Replace("&#34;", "").Replace("&quot;", "") ?? string.Empty;
            if (cardName == string.Empty) cardName = "-----";
            return cardName;
        }
        private static float GetPrice(string priceNode, HtmlNode cardNode) /// if Price 10,3 => 10,30 
        {
            float cardPrice = 0;
            if (cardNode.SelectSingleNode(priceNode) == null) return cardPrice = 0;
            var priceText = cardNode.SelectSingleNode(priceNode).InnerText.Trim().Replace("&nbsp;", "").Replace("р.", "").Replace("руб.", "").Replace("/комплект", "").Replace(".", ",");
            bool isRightPrice = float.TryParse(priceText, NumberStyles.Any, CultureInfo.CurrentCulture, out float price);
            if (isRightPrice) cardPrice = price;
            return cardPrice;
        }
        private static string GetPicture(string pictureNode, string prefixNode, HtmlNode cardNode, string pictureAttribute)
        {
            string? cardPicture = "~/SadClient.jpg";
            if (cardNode.SelectSingleNode(pictureNode) == null) return cardPicture = "~/SadClient.jpg";
            cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == pictureAttribute)?.Value;
            if (cardPicture == "https://turbok.by/img/no-photo--lg.png") return cardPicture = "~/SadClient.jpg";
            if (prefixNode == "https://turbok.by" || prefixNode == "https://minskmagnit.by") return cardPicture;
            if (!String.IsNullOrEmpty(cardPicture))
            {
                cardPicture = string.Concat(prefixNode, cardPicture);
            }
            else
            {
                cardPicture = "~/SadClient.jpg";
            }
            return cardPicture;
        }
        private static string GetCatNumber(string catNumberNode, HtmlNode cardNode)
        {
            string? cardCatNumber = String.Empty;
            if (cardNode.SelectSingleNode(catNumberNode) == null) return cardCatNumber = "-----";
            cardCatNumber = cardNode.SelectSingleNode(catNumberNode)?.InnerText.Trim();
            if (String.IsNullOrEmpty(cardCatNumber)) cardCatNumber = "-----";
            return cardCatNumber;
        }
        private static string GetStatus(string statusNode, HtmlNode cardNode)
        {
            string? cardStatus = "Неизвестный статус";
            if (cardNode.SelectSingleNode(statusNode) == null) return cardStatus = "Неизвестный статус";
            cardStatus = cardNode.SelectSingleNode(statusNode)?.InnerText.Trim();
            if (cardStatus == "Минск")
                cardStatus = cardNode.SelectSingleNode(statusNode)?.Attributes[0].Value.Trim();
            if (cardStatus == "city store-none") return cardStatus = "Под заказ";
            if (cardStatus == "city") return cardStatus = "В наличии";
            if (String.IsNullOrEmpty(cardStatus)) cardStatus = "Неизвестный статус";
            return cardStatus;
        }
        private static bool GetAvailable(string statusNode) => statusNode.ToLower() switch
        {
            "нет в наличии" => false,
            "неизвестный статус" => false,
            "под заказ" => false,
            "в наличии" => true,
            "менее 10 шт" => true,
            _ => false
        };
        private static string GetCardUrl(string cardUrlNode, HtmlNode cardNode)
        {
            string? cardUrl = String.Empty;
            if (cardNode.SelectSingleNode(cardUrlNode) == null) return cardUrl = String.Empty;
            cardUrl = cardNode.SelectSingleNode(cardUrlNode).Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;
            return cardUrl;
        }
        private static string Splite(ref string cardName)
        {
            int charIndexForTrim = 0;
            int numberOfParentheses = 0;
            for (int i = cardName.Length - 1; i >= 0; i--)
            {
                if (cardName[i] == ')')
                {
                    numberOfParentheses++;
                }
                if (cardName[i] == '(')
                    numberOfParentheses--;
                if (numberOfParentheses == 0)
                {
                    charIndexForTrim = i;
                    break;
                }
            }
            var cardNumber = cardName.Remove(cardName.Length - 1)[(charIndexForTrim + 1)..].TrimEnd().Replace("&quot", "");
            if (string.IsNullOrEmpty(cardNumber)) cardNumber = "-----";
            cardName = cardName.Substring(0, charIndexForTrim).Trim().Replace("&quot", "");
            return cardNumber;
        }
        private static string SpliteBelagro(ref string cardName)
        {

            int charIndexForTrim = cardName.IndexOf(' ');
            if (charIndexForTrim <= 0) charIndexForTrim = 0;
            var cardNumber = cardName.Substring(0, charIndexForTrim).Trim();
            if (string.IsNullOrEmpty(cardNumber)) cardNumber = "-----";
            cardName = cardName.Substring(charIndexForTrim).Trim();

            return cardNumber;
        }
    }
}
