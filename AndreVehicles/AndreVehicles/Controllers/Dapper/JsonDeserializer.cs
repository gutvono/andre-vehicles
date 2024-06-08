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
        public ControllerQueries AddressesController { get; set; }
        public ControllerQueries CardsController { get; set; }
        public ControllerQueries CarJobsController { get; set; }
        public ControllerQueries CarsController { get; set; }
        public ControllerQueries CustomersController { get; set; }
        public ControllerQueries EmployeesController { get; set; }
        public ControllerQueries JobsController { get; set; }
        public ControllerQueries PaymentsController { get; set; }
        public ControllerQueries PixesController { get; set; }
        public ControllerQueries PixTypesController { get; set; }
        public ControllerQueries PurchasesController { get; set; }
        public ControllerQueries RolesController { get; set; }
        public ControllerQueries SalesController { get; set; }
        public ControllerQueries TicketsController { get; set; }
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
