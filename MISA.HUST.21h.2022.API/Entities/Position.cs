namespace MISA.HUST._21h._2022.API.Entities
{
    public class Position
    {
        public Guid PositionID { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
