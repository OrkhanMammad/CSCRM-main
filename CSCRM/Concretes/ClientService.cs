using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ClientVMs;
using Microsoft.AspNetCore.Identity;
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

                                                                  }).ToListAsync();
            return ClientsInDb;
        }



        public async Task<BaseResponse> GetAllClientsAsync(short pageIndex)
        {
            try
            {
                List<GetClientVM> ClientsInDb = await GetClients(pageIndex);
                List<string> CompanyNamesInDb = await _context.Companies.Where(c=>c.IsDeleted==false).Select(c=>c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb };
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
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                };

            }
        }
        public async Task<BaseResponse> AddClientAsync(AddClientVM clientVM, AppUser appUser)
        {
            if (clientVM == null || string.IsNullOrEmpty(clientVM.InvCode) || string.IsNullOrEmpty(clientVM.MailCode) || clientVM.ArrivalDate==null || clientVM.ArrivalDate == null)
            {
                List<GetClientVM> ClientsInDb = await GetClients(1);
                List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb };
                var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);
                return new BaseResponse
                {
                    Data = clientsPageMainVm,
                    Message = "Invoice Code, Mail Code, Arrival Date and Departure Date are REQUIRED",
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
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb };
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
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb };
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
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();

                List<GetClientVM> Clients = await GetClients(1);
                List<string> CompanyNames = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMain = new ClientsPageMainVm { Clients = Clients, CompanyNames = CompanyNames };
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
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb };
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
                ClientsPageMainVm clientsPageMain = new ClientsPageMainVm { Clients = Clients, CompanyNames = CompanyNames };
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
                };
                
                EditClientInfoPageMainVM editClientInfoPageMain = new EditClientInfoPageMainVM
                {
                    ClientForUpdate = editClientInfoVM,
                    CompanyNames = CompanyNames,
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
                EditClientInfoPageMainVM editClientInfoPageMain = new EditClientInfoPageMainVM
                {
                    ClientForUpdate = clientVM,
                    CompanyNames = CompanyNames,
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
                        }).ToList()
                    }).FirstOrDefaultAsync(c => c.Id == clientId);
               

                if (clientOrders == null)
                {
                    return new BaseResponse
                    {
                        Data = new GetClientOrdersVM(),
                        Message = "Client Could Not Found By Its Property",
                        StatusCode = "404",
                        Success = false
                    };
                }








                return new BaseResponse
                {
                    Data = clientOrders,
                    StatusCode = "201",
                    Success = true
                };




            }
            catch (Exception ex) 
            {
                return new BaseResponse
                {
                    Data = new GetClientOrdersVM(),
                    Message = "Unhandled Error Ocuured",
                    StatusCode = "500",
                    Success = false
                };
            }

        }


    }
}
