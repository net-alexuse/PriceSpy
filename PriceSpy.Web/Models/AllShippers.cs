﻿namespace PriceSpy.Web.Models
{
    public class Element
    {
        public string? Name { get; set; }
        public string? CatNumber { get; set; }
        public float? Price { get; set; }
    }
    public class Shipper
    {
        public string Name { get; set; }
        public int NumberOfElements
        {
            get { return Elements.Count; }
        }
        public ICollection<Element> Elements { get; set; } = new List<Element>();
        public string PriceFile { get; set; }
        public Shipper(string path, string name)
        {
            PriceFile = path;
            Name = name;
        }
        public IEnumerable<Element> selectedElements { get; set; } = new List<Element>();
    }
    public class AllShippers
    {
        public ICollection<Shipper> shippers { get; set; } = new List<Shipper>();
    }
}