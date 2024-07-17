using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ClientVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.ConfirmationVMs;
using CSCRM.ViewModels.InvoiceVMs;
using CSCRM.ViewModels.TourCarVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace CSCRM.Concretes
{
    public class ClientService : IClientService
    {
        readonly AppDbContext _context;
        readonly UserManager<AppUser> _userManager;

        public ClientService(AppDbContext context)
        {
         _context = context;

        }

       

        private async Task<List<GetClientVM>> GetClients(short pageIndex)
        {
            List<GetClientVM> ClientsInDb = await _context.Clients.Where(c => c.IsDeleted == false)
                                                                  .OrderByDescending(c => c.Id)
                                                                  .Skip((pageIndex - 1) * 6)
                                                                  .Take(6)
                                                                  .Select(c => new GetClientVM
                                                                  {
                                                                          Id = c.Id,
                                                                          InvCode = c.InvCode,
                                                                          MailCode = c.MailCode,
                                                                          Name = c.Name,
                                                                          Surname = c.Surname,
                                                                          SalesAmount = c.SalesAmount,
                                                                          Pending = c.Pending,
                                                                          Received = c.Received,
                                                                          PaySituation = c.PaySituation,
                                                                          VisaSituation = c.VisaSituation,
                                                                          Company = c.Company,
                                                                          Country = c.Country,
                                                                          ArrivalDate = c.ArrivalDate,
                                                                          DepartureDate = c.DepartureDate,
                                                                          ArrivalFlight = c.ArrivalFlight,
                                                                          ArrivalTime = c.ArrivalTime,
                                                                          DepartureFlight = c.DepartureFlight,
                                                                          DepartureTime = c.DepartureTime,
                                                                          PaxSize = c.PaxSize,
                                                                          CarType = c.CarType,

                                                                  }).ToListAsync();
            return ClientsInDb;
        }
        private async Task<GetClientOrdersVM> GetHotelOrdersOfClientAsync(int clientId)
        {
            GetClientOrdersVM clientOrders = await _context.Clients
                     .Include(c => c.HotelOrders)
                     .Select(c => new GetClientOrdersVM
                     {
                         Id = c.Id,
                         InvCode = c.InvCode,
                         MailCode = c.MailCode,
                         Name = c.Name,
                         Surname = c.Surname,
                         HotelOrders = c.HotelOrders.Where(o => !o.IsDeleted).Select(o => new GetHotelOrdersVM
                         {
                             Id = o.Id,
                             ClientId = o.ClientId,
                             HotelName = o.HotelName,
                             RoomType = o.RoomType,
                             RoomCount = o.RoomCount,
                             Days = o.Days,
                             DateFrom = o.DateFrom,
                             DateTo = o.DateTo,

                         }).ToList()
                     }).FirstOrDefaultAsync(c => c.Id == clientId);
            return clientOrders;
        }
        private async Task<GetClientOrdersVM> GetTourOrdersOfClientAsync(int clientId)
        {
            GetClientOrdersVM clientOrders = await _context.Clients
                   .Include(c => c.TourOrders)
                   .Select(c => new GetClientOrdersVM
                   {
                       Id = c.Id,
                       InvCode = c.InvCode,
                       MailCode = c.MailCode,
                       Name = c.Name,
                       Surname = c.Surname,
                       TourOrders = c.TourOrders.Where(o => !o.IsDeleted).Select(o => new GetTourOrdersVM
                       {
                           CarType = o.CarType,
                           TourName = o.Tour.Name,
                           ClientId = o.ClientID,
                           Guide = o.Guide,
                           Date = o.Date,
                           Id = o.Id,
                       }).ToList()
                   }).FirstOrDefaultAsync(c => c.Id == clientId);
            return clientOrders;
        }
        private async Task<GetClientOrdersVM> GetInclusiveOrdersOfClientAsync(int clientId)
        {
            GetClientOrdersVM clientOrders = await _context.Clients
                    .Include(c => c.InclusiveOrders)
                    .Select(c => new GetClientOrdersVM
                    {
                        Id = c.Id,
                        InvCode = c.InvCode,
                        MailCode = c.MailCode,
                        Name = c.Name,
                        Surname = c.Surname,                       
                        InclusiveOrders = c.InclusiveOrders.Where(o => !o.IsDeleted).Select(o => new GetInclusiveOrdersVM
                        {
                            ClientId = o.ClientId,
                            Id = o.Id,
                            InclusiveName = o.InclusiveName,
                            Count = o.Count,
                            Date = o.Date,
                        }).ToList()
                    }).FirstOrDefaultAsync(c => c.Id == clientId);
            return clientOrders;
        }
        private async Task<GetClientOrdersVM> GetRestaurantOrdersOfClientAsync(int clientId)
        {
            GetClientOrdersVM clientOrders = await _context.Clients
                     .Include(c => c.RestaurantOrders)
                     .Select(c => new GetClientOrdersVM
                     {
                         Id = c.Id,
                         InvCode = c.InvCode,
                         MailCode = c.MailCode,
                         Name = c.Name,
                         Surname = c.Surname,                        
                         RestaurantOrders = c.RestaurantOrders.Where(o => !o.IsDeleted).Select(o => new GetRestaurantOrdersVM
                         {
                             Id = o.Id,
                             ClientId = o.ClientID,
                             Count = o.Count,
                             Date = o.Date,
                             MealType = o.MealType,
                             RestaurantName = o.RestaurantName,
                         }).ToList()                        
                     }).FirstOrDefaultAsync(c => c.Id == clientId);
            return clientOrders;
        }




        public async Task<BaseResponse> GetAllClientsAsync(short pageIndex)
        {
            try
            {
                List<GetClientVM> ClientsInDb = await GetClients(pageIndex);
                List<string> CompanyNamesInDb = await _context.Companies.Where(c=>c.IsDeleted==false).Select(c=>c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypes };
                if (!ClientsInDb.Any())
                {
                    return new BaseResponse
                    {
                        Data = clientsPageMainVm,
                        Message = "No Client Found",
                        StatusCode = "404",
                        Success = false,
                    };
                }
                var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                return new BaseResponse
                {
                    Data = clientsPageMainVm,
                    StatusCode = "200",
                    Success = false,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                };
            }
            catch (Exception ex) 
            {

                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    Message = $"Unhandled Error Occured,{ex}",
                    StatusCode = "500",
                    Success = false,
                };

            }
        }
        public async Task<BaseResponse> AddClientAsync(AddClientVM clientVM, AppUser appUser)
        {
            if (clientVM == null || string.IsNullOrEmpty(clientVM.InvCode) || string.IsNullOrEmpty(clientVM.MailCode) || string.IsNullOrWhiteSpace(clientVM.Name) || string.IsNullOrWhiteSpace(clientVM.Surname))
            {
                List<GetClientVM> ClientsInDb = await GetClients(1);
                List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypes };
                var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                return new BaseResponse
                {
                    Data = clientsPageMainVm,
                    Message = "Invoice Code, Mail Code, Client Name and Client Surname are REQUIRED",
                    StatusCode = "400",
                    Success = false,
                    PageIndex = 1,
                    PageSize = pageSize,

                };
            }

            try
            {
                bool InvExists = await _context.Clients.AnyAsync(c => c.InvCode.ToLower() == clientVM.InvCode.Trim().ToLower() && c.IsDeleted == false);
                if (InvExists)
                {
                    List<GetClientVM> ClientsInDb = await GetClients(1);
                    List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    List<string> CarTypesInDb = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypesInDb };
                    var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                    return new BaseResponse
                    {
                        Data = clientsPageMainVm,
                        Message = $"Client By Invoice Code({clientVM.InvCode}) is already exists",
                        StatusCode = "409",
                        Success = false,
                        PageIndex = 1,
                        PageSize = pageSize,

                    };
                }

                bool MailExists = await _context.Clients.AnyAsync(c => c.MailCode.ToLower() == clientVM.MailCode.Trim().ToLower() && c.IsDeleted == false);
                if (MailExists)
                {
                    List<GetClientVM> ClientsInDb = await GetClients(1);
                    List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    List<string> CarTypesInDb = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypesInDb };
                    var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                    return new BaseResponse
                    {
                        Data = clientsPageMainVm,
                        Message = $"Client By Mail Code({clientVM.MailCode}) is already exists",
                        StatusCode = "409",
                        Success = false,
                        PageIndex = 1,
                        PageSize = pageSize,
                    };
                }

                Client newClient = new Client
                {
                    Name = clientVM.Name,
                    Surname = clientVM.Surname,
                    MailCode = clientVM.MailCode,
                    InvCode = clientVM.InvCode,
                    ArrivalDate = clientVM.ArrivalDate,
                    Company = clientVM.Company,
                    Country = clientVM.Country,
                    DepartureDate = clientVM.DepartureDate,
                    PaySituation = clientVM.PaySituation,
                    Pending = clientVM.SalesAmount - clientVM.Received,
                    Received = clientVM.Received,
                    SalesAmount = clientVM.SalesAmount,
                    VisaSituation = clientVM.VisaSituation,
                    IsDeleted = false,
                    CreatedBy = appUser.Name + " " + appUser.SurName,
                    DepartureTime = clientVM.DepartureTime,
                    DepartureFlight = clientVM.DepartureFlight,
                    ArrivalTime = clientVM.ArrivalTime,
                    ArrivalFlight = clientVM.ArrivalFlight,
                    PaxSize = clientVM.PaxSize,
                    CarType = clientVM.CarType,
                };

                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();

                List<GetClientVM> Clients = await GetClients(1);
                List<string> CompanyNames = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMain = new ClientsPageMainVm { Clients = Clients, CompanyNames = CompanyNames, CarTypes = CarTypes };
                var clientsCountInDb = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSizeOfClients = (int)Math.Ceiling((decimal)clientsCountInDb / 6);
                return new BaseResponse
                {
                    Data = clientsPageMain,
                    StatusCode = "201",
                    Message = "New Client Added",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeOfClients,
                };
            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                };

            }
        }
        public async Task<BaseResponse> DeleteClientAsync(int clientId, AppUser appUser)
        {
            try
            {
                Client removingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId && c.IsDeleted==false);
                if (removingClient == null)
                {
                    List<GetClientVM> ClientsInDb = await GetClients(1);
                    List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    List<string> CarTypesInDb = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypesInDb };
                    var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                    return new BaseResponse
                    {
                        Data = clientsPageMainVm,
                        StatusCode = "404",
                        Message = "Client by its property could not found",
                        Success = false,
                        PageIndex = 1,
                        PageSize = pageSize,
                    };
                }

                removingClient.IsDeleted = true;
                removingClient.DeletedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                List<GetClientVM> Clients = await GetClients(1);
                List<string> CompanyNames = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMain = new ClientsPageMainVm { Clients = Clients, CompanyNames = CompanyNames, CarTypes = CarTypes };
                var clientsCountInDb = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSizeOfClients = (int)Math.Ceiling((decimal)clientsCountInDb / 6);
                return new BaseResponse
                {
                    Data = clientsPageMain,
                    StatusCode = "203",
                    Message = "Client Deleted Successfully",
                    Success = true,
                    PageIndex = 1,
                    PageSize = pageSizeOfClients,
                };
            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    StatusCode = "500",
                    Message = "Unhandled error occured",
                    Success = false,                    
                };
            }
        }
        public async Task<BaseResponse> GetClientForEditInfo(int clientId)
        {
            try
            {
                Client clientInDb = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId && c.IsDeleted == false);
                if (clientInDb == null)
                {
                    return new BaseResponse
                    {
                        Data = new EditClientInfoPageMainVM(),
                        Success = false,
                        Message = "Client By Its Property Could Not Found",
                        StatusCode = "404"
                    };
                }
                List<string> CompanyNames = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypesInDb = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();


                EditClientInfoVM editClientInfoVM = new EditClientInfoVM
                {
                    Id = clientInDb.Id,
                    InvCode = clientInDb.InvCode,
                    MailCode = clientInDb.MailCode,
                    Name = clientInDb.Name,
                    Surname = clientInDb.Surname,
                    SalesAmount = clientInDb.SalesAmount,
                    Received = clientInDb.Received,
                    PaySituation = clientInDb.PaySituation,
                    VisaSituation = clientInDb.VisaSituation,
                    Country = clientInDb.Country,
                    Company = clientInDb.Company,
                    ArrivalDate = clientInDb.ArrivalDate,
                    DepartureDate = clientInDb.DepartureDate,
                    ArrivalTime = clientInDb.ArrivalTime,
                    DepartureTime = clientInDb.DepartureTime,
                    ArrivalFlight = clientInDb.ArrivalFlight,
                    DepartureFlight = clientInDb.DepartureFlight,
                    PaxSize = clientInDb.PaxSize,
                    CarType = clientInDb.CarType,
                    
                };
                
                EditClientInfoPageMainVM editClientInfoPageMain = new EditClientInfoPageMainVM
                {
                    ClientForUpdate = editClientInfoVM,
                    CompanyNames = CompanyNames,
                    CarTypes = CarTypesInDb,
                };

                return new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    StatusCode = "200",
                    Success = true,

                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = new EditClientInfoPageMainVM(),
                    Success = false,
                    Message = "Unhandled error occured",
                    StatusCode = "500"
                };            
            }                         
        }
        public async Task<BaseResponse> EditClientInfoAsync(EditClientInfoVM clientVM, AppUser appUser)
        {

            try
            {
                List<string> CompanyNames = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                EditClientInfoPageMainVM editClientInfoPageMain = new EditClientInfoPageMainVM
                {
                    ClientForUpdate = clientVM,
                    CompanyNames = CompanyNames,
                    CarTypes=CarTypes,
                };

                BaseResponse errorResponse = new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    Message = "",
                    StatusCode = "400",
                    Success = false,
                };



                if (clientVM == null || string.IsNullOrEmpty(clientVM.InvCode) || string.IsNullOrEmpty(clientVM.MailCode) || clientVM.ArrivalDate == null || clientVM.ArrivalDate == null)
                {
                    errorResponse.Message = "Invoice Code, Mail Code, Arrival Date and Departure Date are REQUIRED";
                    return errorResponse;
                }



                bool InvExists = await _context.Clients.AnyAsync(c => c.InvCode.ToLower() == clientVM.InvCode.Trim().ToLower() && c.IsDeleted == false && c.Id != clientVM.Id);
                if (InvExists)
                {
                    errorResponse.StatusCode = "409";
                    errorResponse.Message = "Client by this Invoice Code is already exists";
                    return errorResponse;
                }

                bool MailExists = await _context.Clients.AnyAsync(c => c.MailCode.ToLower() == clientVM.MailCode.Trim().ToLower() && c.IsDeleted == false && c.Id != clientVM.Id);
                if (MailExists)
                {
                    errorResponse.StatusCode = "409";
                    errorResponse.Message = "Client by this Mail Code is already exists ";
                    return errorResponse;
                }


                Client updateInfos = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientVM.Id);
                if (updateInfos == null)
                {
                    errorResponse.StatusCode = "404";
                    errorResponse.Message = "Client Could Not Found By Its Property";
                    return errorResponse;
                }

                updateInfos.InvCode = clientVM.InvCode;
                updateInfos.MailCode = clientVM.MailCode;
                updateInfos.Name = clientVM.Name;
                updateInfos.Surname = clientVM.Surname;
                updateInfos.SalesAmount = clientVM.SalesAmount;
                updateInfos.Received= clientVM.Received;
                updateInfos.Pending=clientVM.SalesAmount - clientVM.Received;
                updateInfos.PaySituation= clientVM.PaySituation;
                updateInfos.VisaSituation= clientVM.VisaSituation;
                updateInfos.Company= clientVM.Company;
                updateInfos.Country= clientVM.Country;
                updateInfos.ArrivalDate= clientVM.ArrivalDate;
                updateInfos.DepartureDate= clientVM.DepartureDate;
                updateInfos.UpdatedBy = appUser.Name + " " + appUser.SurName;
                updateInfos.ArrivalFlight= clientVM.ArrivalFlight;
                updateInfos.ArrivalTime= clientVM.ArrivalTime;
                updateInfos.DepartureFlight= clientVM.DepartureFlight;
                updateInfos.DepartureTime= clientVM.DepartureTime;
                updateInfos.PaxSize = clientVM.PaxSize;
                updateInfos.CarType= clientVM.CarType;

                await _context.SaveChangesAsync();
                return new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    Message = "Client's Infos Updated Successfully",
                    StatusCode = "203",
                    Success = true,
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = new EditClientInfoPageMainVM(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                };
            }

        }
        public async Task<BaseResponse> GetClientServicesAsync(int clientId)
        {
            try
            {
               
                GetClientOrdersVM clientOrders = await _context.Clients
                    .Include(c => c.HotelOrders)
                    .Include(c=>c.TourOrders)
                    .Include(c=>c.RestaurantOrders)
                    .Include(c=>c.InclusiveOrders)
                    .Select(c => new GetClientOrdersVM
                    {
                        Id = c.Id,
                        InvCode = c.InvCode,
                        MailCode = c.MailCode,
                        Name = c.Name,
                        Surname = c.Surname,
                        HotelOrders = c.HotelOrders.Where(o => !o.IsDeleted).Select(o => new GetHotelOrdersVM
                        {
                            Id = o.Id,
                            ClientId = o.ClientId,
                            HotelName = o.HotelName,
                            RoomType = o.RoomType,
                            RoomCount = o.RoomCount,
                            Days = o.Days,
                            DateFrom = o.DateFrom,
                            DateTo = o.DateTo,

                        }).ToList(),
                        TourOrders = c.TourOrders.Where(o => !o.IsDeleted).Select(o=> new GetTourOrdersVM
                        {
                            CarType = o.CarType,
                            TourName = o.Tour.Name,
                            ClientId = o.ClientID,
                            Guide = o.Guide,
                            Date = o.Date,
                            Id = o.Id,
                        }).ToList(),
                        RestaurantOrders = c.RestaurantOrders.Where(o => !o.IsDeleted).Select(o=>new GetRestaurantOrdersVM
                        {
                            Id=o.Id,
                            ClientId = o.ClientID,
                            Count = o.Count,
                            Date = o.Date,
                            MealType = o.MealType,
                            RestaurantName = o.RestaurantName,
                        }).ToList(),
                        InclusiveOrders = c.InclusiveOrders.Where(o => !o.IsDeleted).Select(o=> new GetInclusiveOrdersVM
                        {
                            ClientId = o.ClientId,
                            Id = o.Id,
                            InclusiveName = o.InclusiveName,
                            Count = o.Count,
                            Date = o.Date,
                        }).ToList()
                    }).FirstOrDefaultAsync(c => c.Id == clientId);

                if(clientOrders == null)
                {
                    return new BaseResponse
                    {
                        Data = new HotelTourRestaurantInclusiveOrdersTotal(),
                        Message = "Client Could Not Found By Its Property",
                        StatusCode = "404",
                        Success = false,
                    };
                }


                List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                                                                  .Select(t => new GetTourIdNameVM { Id = t.Id, Name = t.Name })    
                                                                  .ToListAsync();

                List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                                                                   .Select(t => new GetCarIdNameVM { Id = t.Id, Name = t.Name })
                                                                   .ToListAsync();

                List<string> HotelNames = await _context.Hotels.Where(t => !t.IsDeleted)
                                                               .Select(t => t.Name)
                                                               .ToListAsync();

                List<string> RestaurantNames = await _context.Restaurants.Where(t => !t.IsDeleted)
                                                                         .Select(t => t.Name)
                                                                         .ToListAsync();

                List<string> InclusiveNames = await _context.InclusiveServices.Where(t => !t.IsDeleted)
                                                                              .Select(t => t.Name)
                                                                              .ToListAsync();




                TourOrdersSectionVM tourOrdersSectionVM = new TourOrdersSectionVM
                {
                    ClientId = clientOrders.Id,
                    Tours= Tours,
                    Cars= Cars,
                };
                
                if (!clientOrders.TourOrders.Any())
                {
                    tourOrdersSectionVM.Success = true;
                    tourOrdersSectionVM.Message = "No Tour Order Found";
                }
                else
                {
                    tourOrdersSectionVM.TourOrders = clientOrders.TourOrders;
   
                }

                HotelOrdersSectionVM hotelOrdersSectionVM = new HotelOrdersSectionVM
                {
                    ClientId = clientOrders.Id,
                    HotelNames = HotelNames,
                };

                if (!clientOrders.HotelOrders.Any())
                {
                    hotelOrdersSectionVM.Success = true;
                    hotelOrdersSectionVM.Message = "No Hotel Order Found";
                }
                else
                {
                    hotelOrdersSectionVM.HotelOrders = clientOrders.HotelOrders;                    
                }

                RestaurantOrdersSectionVM restaurantOrdersSectionVM = new RestaurantOrdersSectionVM
                {
                    ClientId = clientOrders.Id,
                    RestaurantNames = RestaurantNames,
                };
                if (!clientOrders.RestaurantOrders.Any())
                {
                    restaurantOrdersSectionVM.Success = true;
                    restaurantOrdersSectionVM.Message = "No Restaurant Order Found";
                }
                else
                {
                    restaurantOrdersSectionVM.RestaurantOrders = clientOrders.RestaurantOrders;
                }

                InclusiveOrdersSectionVM inclusiveOrdersSectionVM = new InclusiveOrdersSectionVM
                {
                    ClientId = clientOrders.Id,
                    InclusiveNames = InclusiveNames,
                };
                if (!clientOrders.InclusiveOrders.Any())
                {
                    inclusiveOrdersSectionVM.Success = true;
                    inclusiveOrdersSectionVM.Message = "No Inclusive Order Found";
                }
                else
                {
                    inclusiveOrdersSectionVM.InclusiveOrders = clientOrders.InclusiveOrders;
                }


                HotelTourRestaurantInclusiveOrdersTotal hotelTourRestaurantInclusiveOrders = new HotelTourRestaurantInclusiveOrdersTotal
                {
                    ClientId=clientOrders.Id,
                    InvCode = clientOrders.InvCode,
                    MailCode = clientOrders.MailCode,
                    Name = clientOrders.Name,
                    Surname = clientOrders.Surname,
                    HotelOrdersSection = hotelOrdersSectionVM,
                    TourOrdersSection = tourOrdersSectionVM,
                    RestaurantOrdersSection = restaurantOrdersSectionVM,
                    InclusiveOrdersSection = inclusiveOrdersSectionVM,
                };
                return new BaseResponse
                {
                    Data = hotelTourRestaurantInclusiveOrders,
                };


            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new HotelTourRestaurantInclusiveOrdersTotal(),
                    Message = "Unhandled Error Ocuured",
                    StatusCode = "500",
                    Success = false
                    
                };
            }

        }





        public async Task<HotelOrdersSectionVM> DeleteHotelOrderAsync(int clientId, int hotelOrderId, AppUser appUser)
        {
            try
            {
                HotelOrder deletingOrder = await _context.HotelOrders.FirstOrDefaultAsync(h => h.Id == hotelOrderId && h.ClientId == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new HotelOrdersSectionVM
                        {
                            HotelOrders = new List<GetHotelOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, bur order has been deleted successfuly",
                            StatusCode = "404",
                            Success = false,
                            HotelNames = new List<string>()
                        };
                    }
                    else
                    {
                        List<string> HotelNames = await _context.Hotels.Where(t => !t.IsDeleted)
                                                             .Select(t => t.Name)
                                                             .ToListAsync();
                        HotelOrdersSectionVM hotelOrdersSectionVM = new HotelOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            HotelNames = HotelNames,
                            StatusCode = "200",
                            Success = true,
                            HotelOrders = clientOrders.HotelOrders,
                            Message = "Order deleted successfully"
                        };
                        return hotelOrdersSectionVM;

                    }

                }
                else
                {
                    GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new HotelOrdersSectionVM
                        {
                            HotelOrders = new List<GetHotelOrdersVM>(),
                            Message = "Client Could Not Found By Its Property and Order has not been deleted",
                            StatusCode = "404",
                            Success = false,
                            HotelNames = new List<string>()
                        };
                    }

                    else
                    {
                        List<string> HotelNames = await _context.Hotels.Where(t => !t.IsDeleted)
                                                             .Select(t => t.Name)
                                                             .ToListAsync();
                        HotelOrdersSectionVM hotelOrdersSectionVM = new HotelOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            HotelNames = HotelNames,
                            StatusCode = "404",
                            Success = false,
                            HotelOrders = clientOrders.HotelOrders,
                            Message = "Order could not found by its property and has not been deleted"
                        };
                        return hotelOrdersSectionVM;

                    }


                }
            }
            catch (Exception ex) 
            {
                return new HotelOrdersSectionVM
                {
                    HotelOrders = new List<GetHotelOrdersVM>(),
                    Message = "Unhandled error happened",
                    StatusCode = "500",
                    Success = false,
                    HotelNames = new List<string>()
                };
            }
        }
        public async Task<HotelOrdersSectionVM> AddNewHotelOrderAsync(AddNewHotelOrderVM newOrder, AppUser appUser)
        {

            try
            {
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.RoomType) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.HotelName) || newOrder.RoomCount == 0 || newOrder.Days == 0 || string.IsNullOrWhiteSpace(newOrder.FromDate) || string.IsNullOrWhiteSpace(newOrder.ToDate))
                {

                    return new HotelOrdersSectionVM
                    {
                        HotelOrders = new List<GetHotelOrdersVM>(),
                        Message = "Insert datas in a proper way",
                        StatusCode = "500",
                        Success = false,
                        HotelNames = new List<string>()
                    };
                }

                HotelOrder newHotelOrder = new HotelOrder
                {
                    HotelName = newOrder.HotelName,
                    RoomType = newOrder.RoomType,
                    Days = newOrder.Days,
                    ClientId = newOrder.ClientId,
                    DateFrom = newOrder.FromDate,
                    DateTo = newOrder.ToDate,
                    RoomCount = newOrder.RoomCount,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                await _context.HotelOrders.AddAsync(newHotelOrder);
                await _context.SaveChangesAsync();
                GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(newOrder.ClientId);
                if (clientOrders == null)
                {
                    return new HotelOrdersSectionVM
                    {
                        HotelOrders = new List<GetHotelOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, bur order added successfully",
                        StatusCode = "404",
                        Success = false,
                        HotelNames = new List<string>()
                    };
                }
                else
                {
                    List<string> HotelNames = await _context.Hotels.Where(t => !t.IsDeleted)
                                                         .Select(t => t.Name)
                                                         .ToListAsync();
                    HotelOrdersSectionVM hotelOrdersSectionVM = new HotelOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        HotelNames = HotelNames,
                        StatusCode = "200",
                        Success = true,
                        HotelOrders = clientOrders.HotelOrders,
                        Message = "Order added successfully"
                    };
                    return hotelOrdersSectionVM;

                }
            }
            catch (Exception ex)
            {
                return new HotelOrdersSectionVM
                {
                    HotelOrders = new List<GetHotelOrdersVM>(),
                    Message = "Unhandled error occured",
                    StatusCode = "500",
                    Success = false,
                    HotelNames = new List<string>()
                };
            }
           
        }


        public async Task<TourOrdersSectionVM> DeleteTourOrderAsync(int clientId, int tourOrderId, AppUser appUser)
        {
            try
            {
                TourOrder deletingOrder = await _context.TourOrders.FirstOrDefaultAsync(h => h.Id == tourOrderId && h.ClientID == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    GetClientOrdersVM clientOrders = await GetTourOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new TourOrdersSectionVM
                        {
                            TourOrders = new List<GetTourOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfuly",
                            StatusCode = "404",
                            Success = false,
                            Tours = new List<GetTourIdNameVM>(),
                            Cars = new List<GetCarIdNameVM>()
                        };
                    }
                    else
                    {
                        List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                                                             .Select(t => new GetTourIdNameVM
                                                             {
                                                                 Id = t.Id,
                                                                 Name = t.Name,
                                                             })
                                                             .ToListAsync();

                        List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                                                             .Select(t => new GetCarIdNameVM
                                                             {
                                                                 Id = t.Id,
                                                                 Name = t.Name,
                                                             })
                                                             .ToListAsync();

                        TourOrdersSectionVM tourOrdersSectionVM = new TourOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            Tours = Tours,
                            Cars = Cars,
                            StatusCode = "200",
                            Success = true,
                            TourOrders = clientOrders.TourOrders,
                            Message = "Order deleted successfully"
                        };
                        return tourOrdersSectionVM;

                    }

                }
                else
                {
                    GetClientOrdersVM clientOrders = await GetTourOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new TourOrdersSectionVM
                        {
                            TourOrders = new List<GetTourOrdersVM>(),
                            Message = "Client Could Not Found By Its Property and Order has not been deleted",
                            StatusCode = "404",
                            Success = false,
                            Cars = new List<GetCarIdNameVM>(),
                            Tours = new List<GetTourIdNameVM>()
                        };
                    }

                    else
                    {
                        List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                                                             .Select(t => new GetTourIdNameVM
                                                             {
                                                                 Id = t.Id,
                                                                 Name = t.Name,
                                                             })
                                                             .ToListAsync();

                        List<GetCarIdNameVM> Cars = await _context.Tours.Where(t => !t.IsDeleted)
                                                             .Select(t => new GetCarIdNameVM
                                                             {
                                                                 Id = t.Id,
                                                                 Name = t.Name,
                                                             })
                                                             .ToListAsync();
                        TourOrdersSectionVM tourOrdersSectionVM = new TourOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            Tours = Tours,
                            Cars = Cars,
                            StatusCode = "404",
                            Success = false,
                            TourOrders = clientOrders.TourOrders,
                            Message = "Order could not found by its property and has not been deleted"
                        };
                        return tourOrdersSectionVM;

                    }


                }
            }
            catch (Exception ex)
            {
                return new TourOrdersSectionVM
                {
                    TourOrders = new List<GetTourOrdersVM>(),
                    Message = "Unhandled Error Ocuured",
                    StatusCode = "500",
                    Success = false,
                    Cars = new List<GetCarIdNameVM>(),
                    Tours = new List<GetTourIdNameVM>()
                };
            }



        }
        public async Task<TourOrdersSectionVM> AddNewTourOrderAsync(AddNewTourOrderVM newOrder, AppUser appUser)
        {

            try
            {
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.CarType) || newOrder.TourId == 0 || newOrder.Guide == null)
                {

                    return new TourOrdersSectionVM
                    {
                        TourOrders = new List<GetTourOrdersVM>(),
                        Message = "Insert Datas in a Right Way",
                        StatusCode = "404",
                        Success = false,
                        Tours = new List<GetTourIdNameVM>(),
                        Cars = new List<GetCarIdNameVM>()
                    };
                }

                TourOrder newTourOrder = new TourOrder
                {
                    ClientID = newOrder.ClientId,
                    TourId = newOrder.TourId,
                    Guide = newOrder.Guide,
                    CarType = newOrder.CarType,
                    Date = newOrder.Date,
                    CreatedBy = appUser.Name + " " + appUser.SurName,
                };

                await _context.TourOrders.AddAsync(newTourOrder);
                await _context.SaveChangesAsync();
                GetClientOrdersVM clientOrders = await GetTourOrdersOfClientAsync(newOrder.ClientId);
                if (clientOrders == null)
                {
                    return new TourOrdersSectionVM
                    {
                        TourOrders = new List<GetTourOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, bur order added successfully",
                        StatusCode = "404",
                        Success = false,
                        Tours = new List<GetTourIdNameVM>(),
                        Cars = new List<GetCarIdNameVM>()
                    };
                }
                else
                {
                    List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                                                              .Select(t => new GetTourIdNameVM
                                                              {
                                                                  Id = t.Id,
                                                                  Name = t.Name,
                                                              })
                                                              .ToListAsync();

                    List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                                                         .Select(t => new GetCarIdNameVM
                                                         {
                                                             Id = t.Id,
                                                             Name = t.Name,
                                                         })
                                                         .ToListAsync();
                    TourOrdersSectionVM tourOrdersSectionVM = new TourOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        Tours = Tours,
                        Cars = Cars,
                        StatusCode = "200",
                        Success = true,
                        TourOrders = clientOrders.TourOrders,
                        Message = "Order Added Successfully"
                    };
                    return tourOrdersSectionVM;

                }
            }
            catch (Exception ex)
            {
                return new TourOrdersSectionVM
                {
                    TourOrders = new List<GetTourOrdersVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                    Tours = new List<GetTourIdNameVM>(),
                    Cars = new List<GetCarIdNameVM>()
                };
            }

        }


        public async Task<RestaurantOrdersSectionVM> DeleteRestaurantOrderAsync(int clientId, int restaurantOrderId, AppUser appUser)
        {

            try
            {
                RestaurantOrder deletingOrder = await _context.RestaurantOrders.FirstOrDefaultAsync(h => h.Id == restaurantOrderId && h.ClientID == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {

                        return new RestaurantOrdersSectionVM
                        {
                            RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfuly",
                            StatusCode = "404",
                            Success = false,
                            RestaurantNames = new List<string>()
                        };

                    }
                    else
                    {
                        List<string> restaurantNames = await _context.Restaurants.Where(t => !t.IsDeleted)
                                                                        .Select(t => t.Name)
                                                                        .ToListAsync();


                        RestaurantOrdersSectionVM restaurantOrdersSectionVM = new RestaurantOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            RestaurantNames = restaurantNames,
                            RestaurantOrders = clientOrders.RestaurantOrders,
                            StatusCode = "200",
                            Success = true,
                            Message = "Order deleted successfully"
                        };
                        return restaurantOrdersSectionVM;

                    }

                }
                else
                {


                    GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new RestaurantOrdersSectionVM
                        {
                            RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                            Message = "Client Could Not Found By Its Property and Order has not been deleted",
                            StatusCode = "404",
                            Success = false,
                            RestaurantNames = new List<string>()
                        };
                    }

                    else
                    {
                        List<string> restaurantNames = await _context.Restaurants.Where(t => !t.IsDeleted)
                                                                       .Select(t => t.Name)
                                                                       .ToListAsync();


                        RestaurantOrdersSectionVM restaurantOrdersSectionVM = new RestaurantOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            RestaurantNames = restaurantNames,
                            RestaurantOrders = clientOrders.RestaurantOrders,
                            StatusCode = "404",
                            Success = false,
                            Message = "Order could not found by its property and has not been deleted"
                        };
                        return restaurantOrdersSectionVM;

                    }


                }
            }
            catch (Exception ex)
            {
                return new RestaurantOrdersSectionVM
                {
                    RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                    RestaurantNames = new List<string>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                };
            }
        }
        public async Task<RestaurantOrdersSectionVM> AddNewRestaurantOrderAsync(AddNewRestaurantOrderVM newOrder, AppUser appUser)
        {

            try
            {
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.RestaurantName) || string.IsNullOrWhiteSpace(newOrder.MealType) || newOrder.Count <= 0)
                {

                    return new RestaurantOrdersSectionVM
                    {
                        RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                        Message = "Insert Datas in a Right Way",
                        StatusCode = "404",
                        Success = false,
                        RestaurantNames = new List<string>(),
                    };
                }

                RestaurantOrder newRestaurantOrder = new RestaurantOrder
                {
                    ClientID = newOrder.ClientId,
                    Count = newOrder.Count,
                    MealType = newOrder.MealType,
                    Date = newOrder.Date,
                    RestaurantName = newOrder.RestaurantName,
                };

                await _context.RestaurantOrders.AddAsync(newRestaurantOrder);
                await _context.SaveChangesAsync();
                GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(newOrder.ClientId);
                if (clientOrders == null)
                {
                    return new RestaurantOrdersSectionVM
                    {
                        RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, bur order added successfully",
                        StatusCode = "404",
                        Success = false,
                        RestaurantNames = new List<string>(),
                    };
                }
                else
                {

                    List<string> RestaurantNames = await _context.Restaurants.Where(r => !r.IsDeleted).Select(r => r.Name).ToListAsync();
                    return new RestaurantOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        RestaurantOrders = clientOrders.RestaurantOrders,
                        RestaurantNames = RestaurantNames,
                        StatusCode = "200",
                        Success = true,
                        Message = "Order Added Successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RestaurantOrdersSectionVM
                {
                    RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                    RestaurantNames = new List<string>(),
                };
            }

        }


        public async Task<InclusiveOrdersSectionVM> DeleteInclusiveOrderAsync(int clientId, int inclusiveOrderId, AppUser appUser)
        {

            try
            {
                InclusiveOrder deletingOrder = await _context.InclusiveOrders.FirstOrDefaultAsync(h => h.Id == inclusiveOrderId && h.ClientId == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {

                        return new InclusiveOrdersSectionVM
                        {
                            InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfuly",
                            StatusCode = "404",
                            Success = false,
                            InclusiveNames = new List<string>()
                        };

                    }
                    else
                    {
                        List<string> inclusiveNames = await _context.InclusiveServices.Where(t => !t.IsDeleted)
                                                                        .Select(t => t.Name)
                                                                        .ToListAsync();

                        InclusiveOrdersSectionVM inclusiveOrdersSectionVM = new InclusiveOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            InclusiveNames = inclusiveNames,
                            InclusiveOrders = clientOrders.InclusiveOrders,
                            StatusCode = "200",
                            Success = true,
                            Message = "Order deleted successfully"
                        };
                       
                        return inclusiveOrdersSectionVM;
                    }

                }
                else
                {


                    GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new InclusiveOrdersSectionVM
                        {
                            InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                            Message = "Client Could Not Found By Its Property and Order has not been deleted",
                            StatusCode = "404",
                            Success = false,
                            InclusiveNames = new List<string>()
                        };
                    }

                    else
                    {
                        List<string> inclusiveNames = await _context.InclusiveServices.Where(t => !t.IsDeleted)
                                                                       .Select(t => t.Name)
                                                                       .ToListAsync();

                        InclusiveOrdersSectionVM inclusiveOrdersSectionVM = new InclusiveOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            InclusiveNames = inclusiveNames,
                            InclusiveOrders = clientOrders.InclusiveOrders,
                            StatusCode = "404",
                            Success = false,
                            Message = "Order could not found by its property and has not been deleted"
                        };
                        return inclusiveOrdersSectionVM;
                    }


                }
            }
            catch (Exception ex)
            {
                return new InclusiveOrdersSectionVM
                {
                    InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                    InclusiveNames = new List<string>()
                };
            }
        }
        public async Task<InclusiveOrdersSectionVM> AddNewInclusiveOrderAsync(AddNewInclusiveOrderVM newOrder, AppUser appUser)
        {        
            try
            {
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.InclusiveName) || newOrder.Count <= 0)
                {

                    return new InclusiveOrdersSectionVM
                    {
                        InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                        Message = "Insert Datas in a Right Way",
                        StatusCode = "404",
                        Success = false,
                        InclusiveNames = new List<string>(),
                    };
                   
                }

                InclusiveOrder newInclusiveOrder = new InclusiveOrder
                {
                    ClientId = newOrder.ClientId,
                    Count = newOrder.Count,
                    InclusiveName = newOrder.InclusiveName,
                    Date = newOrder.Date,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                await _context.InclusiveOrders.AddAsync(newInclusiveOrder);
                await _context.SaveChangesAsync();
                GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(newOrder.ClientId);
                if (clientOrders == null)
                {
                    return new InclusiveOrdersSectionVM
                    {
                        InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, bur order added successfully",
                        StatusCode = "404",
                        Success = false,
                        InclusiveNames = new List<string>(),
                    };

                }
                else
                {

                    List<string> InclusiveNames = await _context.InclusiveServices.Where(r => !r.IsDeleted).Select(r => r.Name).ToListAsync();
                    return new InclusiveOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        InclusiveOrders = clientOrders.InclusiveOrders,
                        InclusiveNames = InclusiveNames,
                        StatusCode = "200",
                        Success = true,
                        Message = "Order Added Successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                return new InclusiveOrdersSectionVM
                {
                    InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                    InclusiveNames = new List<string>(),
                };
            }
        }

        public async Task<BaseResponse> GetConfirmationAsync(short pageIndex)
        {
            try
            {
                List<GetClientOrdersForConfirmationVM> clientOrders = await _context.Clients
                                                              .Where(c => c.IsDeleted == false)
                                                              .OrderByDescending(c => c.Id)
                                                                  .Skip((pageIndex - 1) * 10)
                                                                  .Take(10)
                                                              .Include(c => c.HotelOrders)
                                                              .Select(c => new GetClientOrdersForConfirmationVM
                                                              {

                                                                  InvCode = c.InvCode,
                                                                  MailCode = c.MailCode,
                                                                  ArrivalDate = c.ArrivalDate,
                                                                  CarType = c.CarType,
                                                                  Country = c.Country,
                                                                  DepartureDate = c.DepartureDate,
                                                                  Guide = true,
                                                                  MarkupTotal = " ",
                                                                  Note = " ",
                                                                  PaxsSize = c.PaxSize,
                                                                  PaymentSituation = c.PaySituation,
                                                                  Pending = c.Pending,
                                                                  Received = c.Received,
                                                                  VisaSituation = c.VisaSituation,
                                                                  CompanyName = c.Company,
                                                                  SalesAmount = c.SalesAmount,
                                                                  HotelOrders = c.HotelOrders.Where(o => !o.IsDeleted).Select(o => new GetHotelOrderForConfirmationVM
                                                                  {
                                                                      HotelName = o.HotelName,
                                                                      FromDate = o.DateFrom,
                                                                      ToDate = o.DateTo,
                                                                  }).ToList()
                                                              }).ToListAsync();
                var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                return clientOrders.Any() 
                ? new BaseResponse { Data = clientOrders, PageIndex = pageIndex, StatusCode = "200", PageSize = pageSize, Success = true }
                : new BaseResponse { Data = clientOrders, PageIndex = 1, PageSize = 1, StatusCode = "404", Success = true, Message = "No Client Found" };

            }
            catch (Exception ex) 
            { 
            return new BaseResponse 
            { 
                Data = new List<GetClientOrdersForConfirmationVM>(),
                Success = false, 
                Message = "Unhandled Error Occured",
                PageIndex = 1, 
                PageSize = 1,
                StatusCode = "500" };
            
            }
           
        }
        public async Task GetVoucherOfClientAsync(int clientId)
        {
            //clientNAME, clientCarType, arrival date, departure date, COMPANYnAME, CompanyContactName, CompanyContactPhone, HotelOrderHotelname,
            //roomType, count, confirmationNo, checkin, checkout, TourOrderTourName, itineraries, inclusiveorderNames





        }

        public async Task<BaseResponse> GetClientByMailOrInvCodeAsync(string code)
        {
            try
            {
                List<GetClientVM> ClientsInDb = await _context.Clients.Where(c => !c.IsDeleted && (c.MailCode.ToLower() == code.Trim().ToLower() || c.InvCode.ToLower() == code.Trim().ToLower()))
                                                                  .Select(c => new GetClientVM
                                                                  {
                                                                      Id = c.Id,
                                                                      InvCode = c.InvCode,
                                                                      MailCode = c.MailCode,
                                                                      Name = c.Name,
                                                                      Surname = c.Surname,
                                                                      SalesAmount = c.SalesAmount,
                                                                      Pending = c.Pending,
                                                                      Received = c.Received,
                                                                      PaySituation = c.PaySituation,
                                                                      VisaSituation = c.VisaSituation,
                                                                      Company = c.Company,
                                                                      Country = c.Country,
                                                                      ArrivalDate = c.ArrivalDate,
                                                                      DepartureDate = c.DepartureDate,
                                                                      ArrivalFlight = c.ArrivalFlight,
                                                                      ArrivalTime = c.ArrivalTime,
                                                                      DepartureFlight = c.DepartureFlight,
                                                                      DepartureTime = c.DepartureTime,
                                                                      PaxSize = c.PaxSize,
                                                                      CarType = c.CarType,

                                                                  }).ToListAsync();
                List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypes };

                if (!ClientsInDb.Any())
                {
                    return new BaseResponse
                    {
                        Data = clientsPageMainVm,
                        Message = "No Client Found",
                        PageIndex = 1,
                        PageSize = 1,
                        StatusCode = "404",
                        Success = false,
                    };
                }
                return new BaseResponse
                {
                    Data = clientsPageMainVm,
                    Success = true,
                    PageIndex = 1,
                    PageSize = 1,
                    StatusCode = "200",
                };




            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    Message = "Unhandled Error Occured",
                    PageIndex = 1,
                    PageSize = 1,
                    StatusCode = "500",
                    Success = false,
                };
            }
        }
    }
}
