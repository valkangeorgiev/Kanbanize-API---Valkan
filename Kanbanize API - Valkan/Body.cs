namespace Kanbanize_API___Valkan
{
    public class Body
    {
        public int? lane_id { get; set; }
        public int? card_id { get; set; }
        public int? column_id { get; set; }
        public string title { get; set; }
        public int? position { get; set; }
        public string color { get; set; }
        public int? priority { get; set; }
    }
    public class BodyWithOppositeVariables
    {
        public string lane_id { get; set; }
        public string card_id { get; set; }
        public string column_id { get; set; }
        public int? title { get; set; }
        public string position { get; set; }
        public int? color { get; set; }
        public string priority { get; set; }
    }
   
}