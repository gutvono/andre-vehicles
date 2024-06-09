using Microsoft.AspNetCore.Mvc;

namespace AndreVehicles.Controllers.Dapper
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public Query Query { get; set; }
    }

    public class Query
    {
        public ControllerQueries Address { get; set; }
        public ControllerQueries Card { get; set; }
        public ControllerQueries CarJob { get; set; }
        public ControllerQueries Car { get; set; }
        public ControllerQueries Customer { get; set; }
        public ControllerQueries Employee { get; set; }
        public ControllerQueries Job { get; set; }
        public ControllerQueries Payment { get; set; }
        public ControllerQueries Pix { get; set; }
        public ControllerQueries PixType { get; set; }
        public ControllerQueries Purchase { get; set; }
        public ControllerQueries Role { get; set; }
        public ControllerQueries Sale { get; set; }
        public ControllerQueries Ticket { get; set; }
    }

    public class ControllerQueries
    {
        public string GET { get; set; }
        public string GETBYID { get; set; }
        public string UPDATE { get; set; }
        public string INSERT { get; set; }
        public string DELETE { get; set; }
        public string EXISTS { get; set; }
    }
}
