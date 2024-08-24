using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ClientVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.ConfirmationVMs;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.InvoiceVMs;
using CSCRM.ViewModels.TourCarVMs;
using CSCRM.ViewModels.VoucherVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Mono.TextTemplating;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CSCRM.Concretes
{
    public class ClientService : IClientService
    {
        readonly AppDbContext _context;
        readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ClientService> _logger;

        public ClientService(AppDbContext context, ILogger<ClientService> logger)
        {
            _context = context;
            _logger = logger;
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
                             ConfirmationNumbers = o.ConfirmationNumbers.Select(cn=>cn.Number).ToList(),

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
            _logger.LogInformation("GetAllClientsAsync method started. PageIndex: {PageIndex}", pageIndex);

            try
            {
                List<GetClientVM> ClientsInDb = await GetClients(pageIndex);
                List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                List<string> CarTypes = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypes };

                if (!ClientsInDb.Any())
                {
                    _logger.LogWarning("No clients found for PageIndex: {PageIndex}", pageIndex);

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

                _logger.LogInformation("Clients retrieved successfully for PageIndex: {PageIndex}", pageIndex);

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
                _logger.LogError(ex, "Unhandled exception occurred in GetAllClientsAsync. PageIndex: {PageIndex}", pageIndex);

                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    Message = $"Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false,
                };
            }
        }

        public async Task<BaseResponse> AddClientAsync(AddClientVM clientVM, AppUser appUser)
        {
            _logger.LogInformation("AddClientAsync method started for user: {UserName}", appUser.Name);

            if (clientVM == null || string.IsNullOrEmpty(clientVM.InvCode) || string.IsNullOrEmpty(clientVM.MailCode) || string.IsNullOrWhiteSpace(clientVM.Name) || string.IsNullOrWhiteSpace(clientVM.Surname))
            {
                _logger.LogWarning("Invalid input detected. Required fields are missing.");

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
                    _logger.LogWarning("Client with Invoice Code {InvCode} already exists.", clientVM.InvCode);

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
                    _logger.LogWarning("Client with Mail Code {MailCode} already exists.", clientVM.MailCode);

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

                _logger.LogInformation("New client added successfully. Client ID: {ClientId}", newClient.Id);

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
                _logger.LogError(ex, "Unhandled exception occurred in AddClientAsync.");

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
                Client removingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId && c.IsDeleted == false);
                if (removingClient == null)
                {
                    List<GetClientVM> ClientsInDb = await GetClients(1);
                    List<string> CompanyNamesInDb = await _context.Companies.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    List<string> CarTypesInDb = await _context.CarTypes.Where(c => c.IsDeleted == false).Select(c => c.Name).ToListAsync();
                    ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm { Clients = ClientsInDb, CompanyNames = CompanyNamesInDb, CarTypes = CarTypesInDb };
                    var clientsCount = await _context.Clients.CountAsync(c => c.IsDeleted == false);
                    int pageSize = (int)Math.Ceiling((decimal)clientsCount / 6);

                    // Loglama eklendi
                    _logger.LogWarning("Client with ID {ClientId} not found", clientId);

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

                // Loglama eklendi
                _logger.LogInformation("Client with ID {ClientId} deleted successfully by {User}", clientId, appUser.Name + " " + appUser.SurName);

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
                // Loglama eklendi
                _logger.LogError(ex, "An unhandled error occurred while deleting client with ID {ClientId}", clientId);

                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred",
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
                    // Loglama eklendi
                    _logger.LogWarning("Client with ID {ClientId} not found", clientId);

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

                // Loglama eklendi
                _logger.LogInformation("Client with ID {ClientId} retrieved successfully for editing", clientId);

                return new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    StatusCode = "200",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                // Loglama eklendi
                _logger.LogError(ex, "An unhandled error occurred while retrieving client with ID {ClientId} for editing", clientId);

                return new BaseResponse
                {
                    Data = new EditClientInfoPageMainVM(),
                    Success = false,
                    Message = "Unhandled error occurred",
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
                    CarTypes = CarTypes,
                };

                BaseResponse errorResponse = new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    Message = "",
                    StatusCode = "400",
                    Success = false,
                };

                if (clientVM == null || string.IsNullOrEmpty(clientVM.InvCode) || string.IsNullOrEmpty(clientVM.MailCode) || clientVM.ArrivalDate == null || clientVM.DepartureDate == null)
                {
                    _logger.LogWarning("Required fields are missing for client ID {ClientId}", clientVM?.Id);

                    errorResponse.Message = "Invoice Code, Mail Code, Arrival Date and Departure Date are REQUIRED";
                    return errorResponse;
                }

                bool InvExists = await _context.Clients.AnyAsync(c => c.InvCode.ToLower() == clientVM.InvCode.Trim().ToLower() && c.IsDeleted == false && c.Id != clientVM.Id);
                if (InvExists)
                {
                    _logger.LogWarning("Client with Invoice Code {InvCode} already exists for client ID {ClientId}", clientVM.InvCode, clientVM.Id);

                    errorResponse.StatusCode = "409";
                    errorResponse.Message = "Client by this Invoice Code already exists";
                    return errorResponse;
                }

                bool MailExists = await _context.Clients.AnyAsync(c => c.MailCode.ToLower() == clientVM.MailCode.Trim().ToLower() && c.IsDeleted == false && c.Id != clientVM.Id);
                if (MailExists)
                {
                    _logger.LogWarning("Client with Mail Code {MailCode} already exists for client ID {ClientId}", clientVM.MailCode, clientVM.Id);

                    errorResponse.StatusCode = "409";
                    errorResponse.Message = "Client by this Mail Code already exists";
                    return errorResponse;
                }

                Client updateInfos = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientVM.Id);
                if (updateInfos == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found", clientVM.Id);

                    errorResponse.StatusCode = "404";
                    errorResponse.Message = "Client Could Not Be Found By Its Property";
                    return errorResponse;
                }

                updateInfos.InvCode = clientVM.InvCode;
                updateInfos.MailCode = clientVM.MailCode;
                updateInfos.Name = clientVM.Name;
                updateInfos.Surname = clientVM.Surname;
                updateInfos.SalesAmount = clientVM.SalesAmount;
                updateInfos.Received = clientVM.Received;
                updateInfos.Pending = clientVM.SalesAmount - clientVM.Received;
                updateInfos.PaySituation = clientVM.PaySituation;
                updateInfos.VisaSituation = clientVM.VisaSituation;
                updateInfos.Company = clientVM.Company;
                updateInfos.Country = clientVM.Country;
                updateInfos.ArrivalDate = clientVM.ArrivalDate;
                updateInfos.DepartureDate = clientVM.DepartureDate;
                updateInfos.UpdatedBy = appUser.Name + " " + appUser.SurName;
                updateInfos.ArrivalFlight = clientVM.ArrivalFlight;
                updateInfos.ArrivalTime = clientVM.ArrivalTime;
                updateInfos.DepartureFlight = clientVM.DepartureFlight;
                updateInfos.DepartureTime = clientVM.DepartureTime;
                updateInfos.PaxSize = clientVM.PaxSize;
                updateInfos.CarType = clientVM.CarType;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Client with ID {ClientId} updated successfully", clientVM.Id);

                return new BaseResponse
                {
                    Data = editClientInfoPageMain,
                    Message = "Client's Info Updated Successfully",
                    StatusCode = "203",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while updating client with ID {ClientId}", clientVM.Id);

                return new BaseResponse
                {
                    Data = new EditClientInfoPageMainVM(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false,
                };
            }
        }

        public async Task<BaseResponse> GetClientServicesAsync(int clientId)
        {
            try
            {
                GetClientOrdersVM clientOrders = await _context.Clients.Where(c => c.Id == clientId)
                    .Include(c => c.HotelOrders)
                    .Include(c => c.TourOrders)
                    .Include(c => c.RestaurantOrders)
                    .Include(c => c.InclusiveOrders)
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
                            ConfirmationNumbers = o.ConfirmationNumbers.Select(co => co.Number).ToList(),

                        }).ToList(),
                        TourOrders = c.TourOrders.Where(o => !o.IsDeleted).Select(o => new GetTourOrdersVM
                        {
                            CarType = o.CarType,
                            TourName = o.Tour.Name,
                            ClientId = o.ClientID,
                            Guide = o.Guide,
                            Date = o.Date,
                            Id = o.Id,
                        }).ToList(),
                        RestaurantOrders = c.RestaurantOrders.Where(o => !o.IsDeleted).Select(o => new GetRestaurantOrdersVM
                        {
                            Id = o.Id,
                            ClientId = o.ClientID,
                            Count = o.Count,
                            Date = o.Date,
                            MealType = o.MealType,
                            RestaurantName = o.RestaurantName,
                        }).ToList(),
                        InclusiveOrders = c.InclusiveOrders.Where(o => !o.IsDeleted).Select(o => new GetInclusiveOrdersVM
                        {
                            ClientId = o.ClientId,
                            Id = o.Id,
                            InclusiveName = o.InclusiveName,
                            Count = o.Count,
                            Date = o.Date,
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if (clientOrders == null)
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
                    Tours = Tours,
                    Cars = Cars,
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
                    ClientId = clientOrders.Id,
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
                // Log the exception (consider using a logger like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while fetching client services.");

                return new BaseResponse
                {
                    Data = new HotelTourRestaurantInclusiveOrdersTotal(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false
                };
            }
        }






        public async Task<HotelOrdersSectionVM> DeleteHotelOrderAsync(int clientId, int hotelOrderId, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Starting to delete hotel order. ClientId: {ClientId}, HotelOrderId: {HotelOrderId}", clientId, hotelOrderId);

                HotelOrder deletingOrder = await _context.HotelOrders.FirstOrDefaultAsync(h => h.Id == hotelOrderId && h.ClientId == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Hotel order deleted successfully. ClientId: {ClientId}, HotelOrderId: {HotelOrderId}", clientId, hotelOrderId);

                    GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client could not be found after deleting the hotel order. ClientId: {ClientId}", clientId);

                        return new HotelOrdersSectionVM
                        {
                            HotelOrders = new List<GetHotelOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfully",
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

                        _logger.LogInformation("Returning hotel orders after successful deletion. ClientId: {ClientId}", clientId);

                        return new HotelOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            HotelNames = HotelNames,
                            StatusCode = "200",
                            Success = true,
                            HotelOrders = clientOrders.HotelOrders,
                            Message = "Order deleted successfully"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning("Hotel order not found or already deleted. ClientId: {ClientId}, HotelOrderId: {HotelOrderId}", clientId, hotelOrderId);

                    GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client could not be found. ClientId: {ClientId}", clientId);

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

                        _logger.LogInformation("Returning hotel orders after failure to delete. ClientId: {ClientId}", clientId);

                        return new HotelOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            HotelNames = HotelNames,
                            StatusCode = "404",
                            Success = false,
                            HotelOrders = clientOrders.HotelOrders,
                            Message = "Order could not found by its property and has not been deleted"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while deleting hotel order. ClientId: {ClientId}, HotelOrderId: {HotelOrderId}", clientId, hotelOrderId);

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
                    _logger.LogWarning("Invalid hotel order data. ClientId: {ClientId}, HotelName: {HotelName}, RoomType: {RoomType}",
                        newOrder.ClientId, newOrder.HotelName, newOrder.RoomType);

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
                    ClientNameSurname = newOrder.ClientNameSurname,
                    HotelName = newOrder.HotelName,
                    RoomType = newOrder.RoomType,
                    Days = newOrder.Days,
                    ClientId = newOrder.ClientId,
                    DateFrom = newOrder.FromDate,
                    DateTo = newOrder.ToDate,
                    RoomCount = newOrder.RoomCount,
                    CreatedBy = appUser.Name + " " + appUser.SurName
                };

                _logger.LogInformation("Adding new hotel order. ClientId: {ClientId}, HotelName: {HotelName}, RoomType: {RoomType}",
                    newOrder.ClientId, newOrder.HotelName, newOrder.RoomType);

                await _context.HotelOrders.AddAsync(newHotelOrder);
                await _context.SaveChangesAsync();

                GetClientOrdersVM clientOrders = await GetHotelOrdersOfClientAsync(newOrder.ClientId);

                if (clientOrders == null)
                {
                    _logger.LogWarning("Client could not be found after adding hotel order. ClientId: {ClientId}", newOrder.ClientId);

                    return new HotelOrdersSectionVM
                    {
                        HotelOrders = new List<GetHotelOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, but order added successfully",
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

                    _logger.LogInformation("Hotel order added successfully. ClientId: {ClientId}", newOrder.ClientId);

                    return new HotelOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        HotelNames = HotelNames,
                        StatusCode = "200",
                        Success = true,
                        HotelOrders = clientOrders.HotelOrders,
                        Message = "Order added successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled error occurred while adding hotel order. ClientId: {ClientId}, HotelName: {HotelName}",
                    newOrder.ClientId, newOrder.HotelName);

                return new HotelOrdersSectionVM
                {
                    HotelOrders = new List<GetHotelOrdersVM>(),
                    Message = "Unhandled error occurred",
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
                _logger.LogInformation("Attempting to delete tour order. ClientId: {ClientId}, TourOrderId: {TourOrderId}", clientId, tourOrderId);

                TourOrder deletingOrder = await _context.TourOrders.FirstOrDefaultAsync(h => h.Id == tourOrderId && h.ClientID == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    _logger.LogInformation("Tour order found. Deleting order. ClientId: {ClientId}, TourOrderId: {TourOrderId}", clientId, tourOrderId);

                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    GetClientOrdersVM clientOrders = await GetTourOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client not found after deleting tour order. ClientId: {ClientId}", clientId);

                        return new TourOrdersSectionVM
                        {
                            TourOrders = new List<GetTourOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfully",
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

                        _logger.LogInformation("Tour order deleted successfully. ClientId: {ClientId}", clientId);

                        return new TourOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            Tours = Tours,
                            Cars = Cars,
                            StatusCode = "200",
                            Success = true,
                            TourOrders = clientOrders.TourOrders,
                            Message = "Order deleted successfully"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning("Tour order not found. ClientId: {ClientId}, TourOrderId: {TourOrderId}", clientId, tourOrderId);

                    GetClientOrdersVM clientOrders = await GetTourOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client could not be found and order was not deleted. ClientId: {ClientId}", clientId);

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

                        List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                                                                .Select(t => new GetCarIdNameVM
                                                                {
                                                                    Id = t.Id,
                                                                    Name = t.Name,
                                                                })
                                                                .ToListAsync();

                        _logger.LogInformation("Order not found but client details are retrieved. ClientId: {ClientId}", clientId);

                        return new TourOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            Tours = Tours,
                            Cars = Cars,
                            StatusCode = "404",
                            Success = false,
                            TourOrders = clientOrders.TourOrders,
                            Message = "Order could not be found by its property and has not been deleted"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while deleting tour order. ClientId: {ClientId}, TourOrderId: {TourOrderId}", clientId, tourOrderId);

                return new TourOrdersSectionVM
                {
                    TourOrders = new List<GetTourOrdersVM>(),
                    Message = "Unhandled Error Occurred",
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
                _logger.LogInformation("Attempting to add a new tour order. ClientId: {ClientId}, TourId: {TourId}", newOrder.ClientId, newOrder.TourId);

                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.CarType) || newOrder.TourId == 0 || newOrder.Guide == null)
                {
                    _logger.LogWarning("Invalid data provided for adding new tour order. NewOrder: {@NewOrder}", newOrder);

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
                    _logger.LogWarning("Client not found after adding tour order. ClientId: {ClientId}", newOrder.ClientId);

                    return new TourOrdersSectionVM
                    {
                        TourOrders = new List<GetTourOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, but order added successfully",
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

                    _logger.LogInformation("Tour order added successfully. ClientId: {ClientId}", newOrder.ClientId);

                    return new TourOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        Tours = Tours,
                        Cars = Cars,
                        StatusCode = "200",
                        Success = true,
                        TourOrders = clientOrders.TourOrders,
                        Message = "Order Added Successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while adding a new tour order. ClientId: {ClientId}, TourId: {TourId}", newOrder.ClientId, newOrder.TourId);

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
                _logger.LogInformation("Attempting to delete restaurant order. ClientId: {ClientId}, RestaurantOrderId: {RestaurantOrderId}", clientId, restaurantOrderId);

                RestaurantOrder deletingOrder = await _context.RestaurantOrders
                    .FirstOrDefaultAsync(h => h.Id == restaurantOrderId && h.ClientID == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Restaurant order deleted successfully. RestaurantOrderId: {RestaurantOrderId}", restaurantOrderId);

                    GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client not found after deleting restaurant order. ClientId: {ClientId}", clientId);

                        return new RestaurantOrdersSectionVM
                        {
                            RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                            Message = "Client Could Not Found By Its Property, but order has been deleted successfully",
                            StatusCode = "404",
                            Success = false,
                            RestaurantNames = new List<string>()
                        };
                    }
                    else
                    {
                        List<string> restaurantNames = await _context.Restaurants
                            .Where(t => !t.IsDeleted)
                            .Select(t => t.Name)
                            .ToListAsync();

                        return new RestaurantOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            RestaurantNames = restaurantNames,
                            RestaurantOrders = clientOrders.RestaurantOrders,
                            StatusCode = "200",
                            Success = true,
                            Message = "Order deleted successfully"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning("Restaurant order not found for deletion. RestaurantOrderId: {RestaurantOrderId}", restaurantOrderId);

                    GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        _logger.LogWarning("Client not found. ClientId: {ClientId}", clientId);

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
                        List<string> restaurantNames = await _context.Restaurants
                            .Where(t => !t.IsDeleted)
                            .Select(t => t.Name)
                            .ToListAsync();

                        return new RestaurantOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            RestaurantNames = restaurantNames,
                            RestaurantOrders = clientOrders.RestaurantOrders,
                            StatusCode = "404",
                            Success = false,
                            Message = "Order could not be found by its property and has not been deleted"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred while deleting restaurant order. ClientId: {ClientId}, RestaurantOrderId: {RestaurantOrderId}", clientId, restaurantOrderId);

                return new RestaurantOrdersSectionVM
                {
                    RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                    RestaurantNames = new List<string>(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false
                };
            }
        }

        public async Task<RestaurantOrdersSectionVM> AddNewRestaurantOrderAsync(AddNewRestaurantOrderVM newOrder, AppUser appUser)
        {
            try
            {
                // Log the attempt to add a new restaurant order
                _logger.LogInformation("Attempting to add new restaurant order. ClientId: {ClientId}, RestaurantName: {RestaurantName}, Date: {Date}",
                                        newOrder.ClientId, newOrder.RestaurantName, newOrder.Date);

                // Validate the input
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 ||
                    string.IsNullOrWhiteSpace(newOrder.RestaurantName) || string.IsNullOrWhiteSpace(newOrder.MealType) ||
                    newOrder.Count <= 0)
                {
                    _logger.LogWarning("Invalid data provided for new restaurant order. ClientId: {ClientId}, RestaurantName: {RestaurantName}",
                                        newOrder.ClientId, newOrder.RestaurantName);

                    return new RestaurantOrdersSectionVM
                    {
                        RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                        Message = "Insert Data in a Right Way",
                        StatusCode = "404",
                        Success = false,
                        RestaurantNames = new List<string>(),
                    };
                }

                // Create and save the new restaurant order
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

                // Retrieve updated client orders
                GetClientOrdersVM clientOrders = await GetRestaurantOrdersOfClientAsync(newOrder.ClientId);

                if (clientOrders == null)
                {
                    _logger.LogWarning("Client not found after adding restaurant order. ClientId: {ClientId}", newOrder.ClientId);

                    return new RestaurantOrdersSectionVM
                    {
                        RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                        Message = "Client Could Not Found By Its Property, but order added successfully",
                        StatusCode = "404",
                        Success = false,
                        RestaurantNames = new List<string>(),
                    };
                }
                else
                {
                    // Retrieve list of restaurant names
                    List<string> restaurantNames = await _context.Restaurants
                        .Where(r => !r.IsDeleted)
                        .Select(r => r.Name)
                        .ToListAsync();

                    return new RestaurantOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        RestaurantOrders = clientOrders.RestaurantOrders,
                        RestaurantNames = restaurantNames,
                        StatusCode = "200",
                        Success = true,
                        Message = "Order Added Successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Unhandled error occurred while adding new restaurant order. ClientId: {ClientId}, RestaurantName: {RestaurantName}",
                                 newOrder.ClientId, newOrder.RestaurantName);

                return new RestaurantOrdersSectionVM
                {
                    RestaurantOrders = new List<GetRestaurantOrdersVM>(),
                    Message = "Unhandled Error Occurred",
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
                // Log the attempt to delete an inclusive order
                _logger.LogInformation("Attempting to delete inclusive order. ClientId: {ClientId}, InclusiveOrderId: {InclusiveOrderId}",
                                        clientId, inclusiveOrderId);

                // Retrieve the inclusive order to delete
                InclusiveOrder deletingOrder = await _context.InclusiveOrders
                    .FirstOrDefaultAsync(h => h.Id == inclusiveOrderId && h.ClientId == clientId && !h.IsDeleted);

                if (deletingOrder != null)
                {
                    // Mark the order as deleted
                    deletingOrder.IsDeleted = true;
                    deletingOrder.DeletedBy = appUser.Name + " " + appUser.SurName;
                    await _context.SaveChangesAsync();

                    // Retrieve updated client orders
                    GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        // Log warning if the client could not be found
                        _logger.LogWarning("Client not found after deleting inclusive order. ClientId: {ClientId}", clientId);

                        return new InclusiveOrdersSectionVM
                        {
                            InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                            Message = "Client Could Not Be Found By Its Property, but order has been deleted successfully",
                            StatusCode = "404",
                            Success = false,
                            InclusiveNames = new List<string>()
                        };
                    }
                    else
                    {
                        // Retrieve list of inclusive names
                        List<string> inclusiveNames = await _context.InclusiveServices
                            .Where(t => !t.IsDeleted)
                            .Select(t => t.Name)
                            .ToListAsync();

                        return new InclusiveOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            InclusiveNames = inclusiveNames,
                            InclusiveOrders = clientOrders.InclusiveOrders,
                            StatusCode = "200",
                            Success = true,
                            Message = "Order deleted successfully"
                        };
                    }
                }
                else
                {
                    // Log warning if the order to delete was not found
                    _logger.LogWarning("Inclusive order not found for deletion. ClientId: {ClientId}, InclusiveOrderId: {InclusiveOrderId}",
                                        clientId, inclusiveOrderId);

                    GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(clientId);

                    if (clientOrders == null)
                    {
                        return new InclusiveOrdersSectionVM
                        {
                            InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                            Message = "Client Could Not Be Found By Its Property and Order Has Not Been Deleted",
                            StatusCode = "404",
                            Success = false,
                            InclusiveNames = new List<string>()
                        };
                    }
                    else
                    {
                        List<string> inclusiveNames = await _context.InclusiveServices
                            .Where(t => !t.IsDeleted)
                            .Select(t => t.Name)
                            .ToListAsync();

                        return new InclusiveOrdersSectionVM
                        {
                            ClientId = clientOrders.Id,
                            InclusiveNames = inclusiveNames,
                            InclusiveOrders = clientOrders.InclusiveOrders,
                            StatusCode = "404",
                            Success = false,
                            Message = "Order Could Not Be Found By Its Property and Has Not Been Deleted"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Unhandled error occurred while deleting inclusive order. ClientId: {ClientId}, InclusiveOrderId: {InclusiveOrderId}",
                                 clientId, inclusiveOrderId);

                return new InclusiveOrdersSectionVM
                {
                    InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                    InclusiveNames = new List<string>(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false,
                };
            }
        }

        public async Task<InclusiveOrdersSectionVM> AddNewInclusiveOrderAsync(AddNewInclusiveOrderVM newOrder, AppUser appUser)
        {
            try
            {
                // Log the attempt to add a new inclusive order
                _logger.LogInformation("Attempting to add new inclusive order. ClientId: {ClientId}, InclusiveName: {InclusiveName}, Date: {Date}",
                                        newOrder.ClientId, newOrder.InclusiveName, newOrder.Date);

                // Validate input data
                if (newOrder == null || string.IsNullOrWhiteSpace(newOrder.Date) || newOrder.ClientId == 0 || string.IsNullOrWhiteSpace(newOrder.InclusiveName) || newOrder.Count <= 0)
                {
                    _logger.LogWarning("Invalid data provided for adding new inclusive order. ClientId: {ClientId}, InclusiveName: {InclusiveName}",
                                        newOrder.ClientId, newOrder.InclusiveName);

                    return new InclusiveOrdersSectionVM
                    {
                        InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                        Message = "Insert Data in the Right Way",
                        StatusCode = "404",
                        Success = false,
                        InclusiveNames = new List<string>(),
                    };
                }

                // Create a new inclusive order
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

                // Retrieve updated client orders
                GetClientOrdersVM clientOrders = await GetInclusiveOrdersOfClientAsync(newOrder.ClientId);

                if (clientOrders == null)
                {
                    // Log warning if the client could not be found
                    _logger.LogWarning("Client not found after adding new inclusive order. ClientId: {ClientId}", newOrder.ClientId);

                    return new InclusiveOrdersSectionVM
                    {
                        InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                        Message = "Client Could Not Be Found By Its Property, but order has been added successfully",
                        StatusCode = "404",
                        Success = false,
                        InclusiveNames = new List<string>(),
                    };
                }
                else
                {
                    // Retrieve list of inclusive names
                    List<string> inclusiveNames = await _context.InclusiveServices
                        .Where(r => !r.IsDeleted)
                        .Select(r => r.Name)
                        .ToListAsync();

                    return new InclusiveOrdersSectionVM
                    {
                        ClientId = clientOrders.Id,
                        InclusiveOrders = clientOrders.InclusiveOrders,
                        InclusiveNames = inclusiveNames,
                        StatusCode = "200",
                        Success = true,
                        Message = "Order Added Successfully"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Unhandled error occurred while adding new inclusive order. ClientId: {ClientId}, InclusiveName: {InclusiveName}, Date: {Date}",
                                 newOrder.ClientId, newOrder.InclusiveName, newOrder.Date);

                return new InclusiveOrdersSectionVM
                {
                    InclusiveOrders = new List<GetInclusiveOrdersVM>(),
                    InclusiveNames = new List<string>(),
                    Message = "Unhandled Error Occurred",
                    StatusCode = "500",
                    Success = false,
                };
            }
        }


        public async Task<BaseResponse> GetConfirmationAsync(short pageIndex)
        {
            try
            {
                // Log the attempt to fetch client orders for confirmation
                _logger.LogInformation("Fetching client orders for confirmation. PageIndex: {PageIndex}", pageIndex);

                // Retrieve client orders with pagination
                List<GetClientOrdersForConfirmationVM> clientOrders = await _context.Clients
                    .Where(c => !c.IsDeleted)
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
                        HotelOrders = c.HotelOrders
                            .Where(o => !o.IsDeleted)
                            .Select(o => new GetHotelOrderForConfirmationVM
                            {
                                HotelName = o.HotelName,
                                FromDate = o.DateFrom,
                                ToDate = o.DateTo,
                            })
                            .ToList()
                    })
                    .ToListAsync();

                // Calculate the total number of pages based on client count
                var clientsCount = await _context.Clients.CountAsync(c => !c.IsDeleted);
                int pageSize = (int)Math.Ceiling((decimal)clientsCount / 10); // Changed from 6 to 10 to match the page size used in Skip and Take

                // Check if any client orders were found
                if (clientOrders.Any())
                {
                    _logger.LogInformation("Client orders fetched successfully. PageIndex: {PageIndex}, TotalClients: {ClientsCount}", pageIndex, clientsCount);
                    return new BaseResponse
                    {
                        Data = clientOrders,
                        PageIndex = pageIndex,
                        StatusCode = "200",
                        PageSize = pageSize,
                        Success = true
                    };
                }
                else
                {
                    _logger.LogInformation("No client orders found for the given page index. PageIndex: {PageIndex}", pageIndex);
                    return new BaseResponse
                    {
                        Data = clientOrders,
                        PageIndex = 1,
                        PageSize = 1,
                        StatusCode = "404",
                        Success = true,
                        Message = "No Client Found"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Unhandled error occurred while fetching client orders for confirmation. PageIndex: {PageIndex}", pageIndex);

                return new BaseResponse
                {
                    Data = new List<GetClientOrdersForConfirmationVM>(),
                    Success = false,
                    Message = "Unhandled Error Occurred",
                    PageIndex = 1,
                    PageSize = 1,
                    StatusCode = "500"
                };
            }
        }

        public async Task<BaseResponse> GetVoucherOfClientAsync(int clientId)
        {
            try
            {
                // Log the attempt to fetch voucher information for the client
                _logger.LogInformation("Fetching voucher information for client with ID: {ClientId}", clientId);

                // Retrieve client orders with related entities
                GetClientOrdersForVoucherVM clientOrders = await _context.Clients
                    .Where(c => c.Id == clientId)
                    .Include(c => c.HotelOrders)
                    .Include(c => c.TourOrders)
                    .ThenInclude(to => to.Tour)
                    .ThenInclude(t => t.Itineraries)
                    .Include(c => c.InclusiveOrders)
                    .Select(c => new GetClientOrdersForVoucherVM
                    {
                        ClientName = c.Name,
                        ClientSurname = c.Surname,
                        ClientPaxSize = c.PaxSize,
                        ClientCar = c.CarType,
                        ArrivalDate = c.ArrivalDate,
                        DepartureDate = c.DepartureDate,
                        CompanyName = c.Company,
                        InclusiveOrderNames = c.InclusiveOrders
                            .Where(io => !io.IsDeleted)
                            .Select(io => io.InclusiveName)
                            .ToList(),
                        HotelOrders = c.HotelOrders
                            .Where(ho => !ho.IsDeleted)
                            .Select(ho => new GetHotelOrderForVoucherVM
                            {
                                Count = ho.RoomCount,
                                HotelName = ho.HotelName,
                                RoomType = ho.RoomType,
                                FromDate = ho.DateFrom,
                                ToDate = ho.DateTo,
                                ConfirmationNumbers = ho.ConfirmationNumbers
                                    .Select(cn => cn.Number)
                                    .ToList()
                            })
                            .ToList(),
                        TourOrders = c.TourOrders
                            .Where(to => !to.IsDeleted)
                            .Select(to => new GetTourOrderForVoucherVM
                            {
                                Date = to.Date,
                                Tour = new GetTourForVoucherVM
                                {
                                    TourName = to.Tour.Name,
                                    Itineraries = to.Tour.Itineraries
                                        .Select(i => i.Description)
                                        .ToList()
                                }
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (clientOrders == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found", clientId);
                    return new BaseResponse
                    {
                        Data = new GetClientOrdersForVoucherVM(),
                        Message = "Client could not be found by its property",
                        StatusCode = "404",
                        Success = false
                    };
                }

                // Retrieve company contact information
                GetCompanyForVoucherVM company = await _context.Companies
                    .Where(c => c.Name == clientOrders.CompanyName)
                    .Select(c => new GetCompanyForVoucherVM
                    {
                        ContactPerson = c.ContactPerson,
                        ContactPhone = c.Phone
                    })
                    .FirstOrDefaultAsync();

                if (company != null)
                {
                    clientOrders.CompanyContactPerson = company.ContactPerson;
                    clientOrders.CompanyContactPhone = company.ContactPhone;
                }
                else
                {
                    _logger.LogWarning("Company with name {CompanyName} not found", clientOrders.CompanyName);
                }

                return new BaseResponse
                {
                    Data = clientOrders,
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "Unhandled error occurred while fetching voucher information for client with ID: {ClientId}", clientId);

                return new BaseResponse
                {
                    Data = new GetClientOrdersForVoucherVM(),
                    Success = false,
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }


        public async Task<BaseResponse> GetClientByMailOrInvCodeAsync(string code)
        {
            try
            {
                // Normalize input code
                var normalizedCode = code.Trim().ToLower();

                // Fetch clients based on MailCode or InvCode
                List<GetClientVM> clientsInDb = await _context.Clients
                    .Where(c => !c.IsDeleted && (c.MailCode.ToLower() == normalizedCode || c.InvCode.ToLower() == normalizedCode))
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

                // Fetch company names and car types
                List<string> companyNamesInDb = await _context.Companies
                    .Where(c => !c.IsDeleted)
                    .Select(c => c.Name)
                    .ToListAsync();

                List<string> carTypes = await _context.CarTypes
                    .Where(c => !c.IsDeleted)
                    .Select(c => c.Name)
                    .ToListAsync();

                // Prepare the view model
                ClientsPageMainVm clientsPageMainVm = new ClientsPageMainVm
                {
                    Clients = clientsInDb,
                    CompanyNames = companyNamesInDb,
                    CarTypes = carTypes
                };

                if (!clientsInDb.Any())
                {
                    _logger.LogInformation("No client found for code: {Code}", code);
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

                _logger.LogInformation("Clients found for code: {Code}", code);
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
                _logger.LogError(ex, "Unhandled error occurred while fetching client by MailCode or InvCode: {Code}", code);
                return new BaseResponse
                {
                    Data = new ClientsPageMainVm(),
                    Message = "Unhandled Error Occurred",
                    PageIndex = 1,
                    PageSize = 1,
                    StatusCode = "500",
                    Success = false,
                };
            }
        }




            //Updates starts here
            //Updates starts here
            //Updates starts here
            //Updates starts here

        public async Task<BaseResponse> GetHotelOrderByIdAsync(int hotelOrderId)
        {
            try 
            {
                _logger.LogInformation("Attempting to retrieve hotel order with ID {HotelOrderId}.", hotelOrderId);

                EditHotelOrderVM hotelOrder = await _context.HotelOrders.Where(ho=>ho.Id==hotelOrderId ).Include(ho => ho.ConfirmationNumbers).Select(ho => new EditHotelOrderVM
                {
                    HotelOrderId = ho.Id,
                    HotelName = ho.HotelName,
                    RoomType = ho.RoomType,
                    RoomCount = ho.RoomCount,
                    Days = ho.Days,
                    DateFrom = ho.DateFrom,
                    DateTo = ho.DateTo,
                    HotelNames = new List<string>()
                }).FirstOrDefaultAsync();

                if (hotelOrder == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelOrderId} not found.", hotelOrderId);
                    return new BaseResponse
                    {
                        Message = "Hotel Order Could Not Be Found",
                        StatusCode = "404",
                        Success = false,
                        Data = new EditHotelOrderVM()
                    };
                }

                hotelOrder.HotelNames = await _context.Hotels.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();



                _logger.LogInformation("Hotel Order with ID {HotelOrderId} retrieved successfully.", hotelOrderId);
                return new BaseResponse
                {
                    Success = true,
                    Data = hotelOrder,
                    StatusCode = "201",
                    

                };
            }
            
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving hotel order with ID {HotelOrderId}.", hotelOrderId);
                return new BaseResponse
                {
                    Success = false,
                    Data = new EditHotelOrderVM(),
                    StatusCode = "500",
                    Message = "Unhandled error occurred"
                };
            }
        }

        public async Task<BaseResponse> EditHotelOrderAsync(EditHotelOrderVM hotelOrderVM, AppUser appUser)
        {
            if (hotelOrderVM == null || hotelOrderVM.HotelOrderId <= 0)
            {
                _logger.LogWarning("Invalid Hotel Order ID.");
                return new BaseResponse
                {
                    Success = false,
                    Message = "Invalid Hotel Order ID.",
                    StatusCode = "400",
                    Data = hotelOrderVM
                };
            }

            try
            {
                _logger.LogInformation("Retrieving Hotel Order with ID {HotelOrderId}.", hotelOrderVM.HotelOrderId);
                var hotelOrder = await _context.HotelOrders.FirstOrDefaultAsync(h => h.Id == hotelOrderVM.HotelOrderId);

                if (hotelOrder == null)
                {
                    _logger.LogWarning("Hotel Order with ID {HotelOrderId} not found.", hotelOrderVM.HotelOrderId);
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "Hotel Order not found.",
                        StatusCode = "404",
                        Data = hotelOrderVM
                    };
                }


                hotelOrder.HotelName = hotelOrderVM.HotelName ?? hotelOrder.HotelName;
                hotelOrder.RoomType = hotelOrderVM.RoomType ?? hotelOrder.RoomType;
                hotelOrder.RoomCount = hotelOrderVM.RoomCount;
                hotelOrder.Days = hotelOrderVM.Days;
                hotelOrder.DateFrom = hotelOrderVM.DateFrom ?? hotelOrder.DateFrom;
                hotelOrder.DateTo = hotelOrderVM.DateTo ?? hotelOrder.DateTo;
                hotelOrder.UpdatedBy = appUser.Name + " " + appUser.SurName;

                _context.HotelOrders.Update(hotelOrder);
                await _context.SaveChangesAsync();


                var responseVM = new EditHotelOrderVM
                {
                    HotelOrderId = hotelOrder.Id,
                    HotelName = hotelOrder.HotelName,
                    RoomType = hotelOrder.RoomType,
                    RoomCount = hotelOrder.RoomCount,
                    Days = hotelOrder.Days,
                    DateFrom = hotelOrder.DateFrom,
                    DateTo = hotelOrder.DateTo,
                    HotelNames = new List<string>()
                };
                responseVM.HotelNames = await _context.Hotels.Where(h => h.IsDeleted == false).Select(h => h.Name).ToListAsync();

                return new BaseResponse
                {
                    Data = responseVM,
                    Message = "Hotel Order updated successfully.",
                    Success = true,
                    StatusCode = "200"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the hotel order with ID {HotelOrderId}.", hotelOrderVM.HotelOrderId);
                return new BaseResponse
                {
                    Data = hotelOrderVM,
                    Success = false,
                    Message = "An error occurred while updating the hotel order.",
                    StatusCode = "500"
                };
            }
        }


        public async Task<BaseResponse> GetTourOrderByIdAsync(int tourOrderId)
        {
            try
            {
                EditTourOrderVM tourOrder = await _context.TourOrders.Where(to => !to.IsDeleted && to.Id == tourOrderId).Select(to => new EditTourOrderVM
                {
                    Id = to.Id,
                    CarType = to.CarType,
                    Guide = to.Guide,
                    Date = to.Date,
                    TourId = to.Tour.Id
                }).FirstOrDefaultAsync();

                List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                        .Select(t => new GetTourIdNameVM { Id = t.Id, Name = t.Name })
                        .ToListAsync();

                List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                    .Select(t => new GetCarIdNameVM { Id = t.Id, Name = t.Name })
                    .ToListAsync();

                return new BaseResponse
                {

                    Data = new EditTourOrderPageMainVm { Cars = Cars, TourOrder = tourOrder, Tours = Tours },
                    StatusCode = "200",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tour order with ID {TourOrderId}.", tourOrderId);
                return new BaseResponse
                {
                    Data = new EditTourOrderPageMainVm(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false
                };
            }    
        }

        public async Task<BaseResponse> EditTourOrderAsync(EditTourOrderVM tourOrder, AppUser appUser)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve tour order with ID {TourOrderId}.", tourOrder.Id);
                TourOrder order = await _context.TourOrders.FirstOrDefaultAsync(to => to.Id == tourOrder.Id);
                if (order == null) 
                {
                    _logger.LogWarning("Order Could Not Found By Its Property", tourOrder.Id);
                    return new BaseResponse
                    {
                        Data = new EditTourOrderPageMainVm(),
                        Message = "Order Could Not Found By Its Property",
                        StatusCode = "404",
                        Success = false
                    };
                }

                order.TourId = tourOrder.Id;
                order.CarType = tourOrder.CarType;
                order.Guide = tourOrder.Guide;
                order.Date = tourOrder.Date;
                order.UpdatedBy = appUser.Name + " " + appUser.SurName;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order Updated Successfully", tourOrder.Id);

                List<GetTourIdNameVM> Tours = await _context.Tours.Where(t => !t.IsDeleted)
                        .Select(t => new GetTourIdNameVM { Id = t.Id, Name = t.Name })
                        .ToListAsync();

                List<GetCarIdNameVM> Cars = await _context.CarTypes.Where(t => !t.IsDeleted)
                    .Select(t => new GetCarIdNameVM { Id = t.Id, Name = t.Name })
                    .ToListAsync();

                return new BaseResponse
                {

                    Data = new EditTourOrderPageMainVm { Cars = Cars, TourOrder = tourOrder, Tours = Tours },
                    StatusCode = "200",
                    Success = true,
                    Message = "Tour Order Updated Successfully"
                };



            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                return new BaseResponse
                {
                    Data = new EditTourOrderPageMainVm(),
                    Message = "Unhandled Error Occured",
                    StatusCode = "500",
                    Success = false
                };


            }
        }
    }
}
