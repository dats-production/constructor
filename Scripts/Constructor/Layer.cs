using System.Collections.Generic;
using Constructor.Details;

namespace Constructor
{
    public class Layer
    {
        public string Name { get; set; }
        public List<Detail> Details { get; set; } = new List<Detail>();

        public Layer(string name)
        {
            Name = name;
        }
    
        public void AddDetail(Detail detail)
        {
            Details.Add(detail);
            Details.Sort();
        }
    }
}
