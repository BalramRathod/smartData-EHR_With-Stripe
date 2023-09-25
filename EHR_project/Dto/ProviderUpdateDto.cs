namespace EHR_project.Dto
{
    public class ProviderUpdateDto
    {

       
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public int? Experience { get; set; }
        public int? Speciality { get; set; }
        public string? Position { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Fees { get; set; }
    }
}
