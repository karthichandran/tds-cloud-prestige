using System;
using AutoMapper;
using MediatR;
using ReProServices.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReProServices.Domain.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ReProServices.Application.Customers.Queries
{
   public class GetCustomerReportQuery : IRequest<ICollection<ViewCustomerReportModel>>
    {
        public CustomerDetailsFilter Filter { get; set; } = new CustomerDetailsFilter();
        public class GetCustomerReportQuerytHandler : IRequestHandler<GetCustomerReportQuery, ICollection<ViewCustomerReportModel>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<GetCustomerReportQuerytHandler> _logger;
            public GetCustomerReportQuerytHandler(IApplicationDbContext context, IMapper mapper, ILogger<GetCustomerReportQuerytHandler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<ICollection<ViewCustomerReportModel>> Handle(GetCustomerReportQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter;

                var qry = "exec sp_CustomerReport " ;
                qry += string.IsNullOrEmpty(filter.CustomerName) ? "' '" : "'" + filter.CustomerName + "'";
                qry += string.IsNullOrEmpty(filter.PAN) ? ",' '" : ",'" + filter.PAN.ToUpper() + "'";
                qry += filter.PropertyId <= 0 ? ",0" : "," + filter.PropertyId;
                qry += string.IsNullOrEmpty(filter.Premises) ? ",' '" : ",'" + filter.Premises + "'";
                qry += string.IsNullOrEmpty(filter.UnitNo ) ? ",' '" : ",'" + filter.UnitNo + "'";
                qry += filter.StatusTypeId <= 0 ? ",0" : "," + filter.StatusTypeId;
                qry += string.IsNullOrEmpty(filter.Remarks) ? ",' '" : ",'" + filter.Remarks + "'";

                var vm = _context.ViewCustomerReports.FromSqlRaw(qry).ToList();

                var final= vm
                     .GroupBy(g => g.OwnershipID)
                   .Select(x => new ViewCustomerReportModel
                   {
                       PropertyPremises = x.First().PropertyPremises,
                       CustomerName = string.Join(",", x.Select(g => g.CustomerName)),
                       PAN = string.Join(",", x.Select(g => g.PAN)),
                       DateOfAgreement = x.First().DateOfAgreement,
                       OwnershipID = x.First().OwnershipID,
                       UnitNo = x.First().UnitNo,
                       CustomerID = x.First().CustomerID,
                       CustomerPropertyID = x.First().CustomerPropertyID,
                       DateOfSubmission = x.First().DateOfSubmission,
                       PaymentMethodId = x.First().PaymentMethodId,
                       PropertyID = x.First().PropertyID,
                       //  Remarks = x.First().Remarks,
                       // Remarks = string.Join(",", x.Select(g => formatRemarks(g))),
                       Remarks =GetRemarks(x.ToList()),
                       StatusTypeID = x.First().StatusTypeID,
                       TotalUnitCost = x.First().TotalUnitCost,
                       TracesPassword = string.Join(",", x.Select(g => g.TracesPassword)),
                       CustomerAlias = x.First().CustomerAlias,
                       UnitStatus = x.First().UnitStatus,
                       IsPanVerified = string.Join(",", x.Select(g => g.IsPanVerified)),
                       StampDuty = x.First().StampDuty,
                       CustomerStatus= string.Join(",", x.Select(g => g.CustomerStatus)),
                       IncomeTaxPassword= string.Join(",", x.Select(g => g.IncomeTaxPassword)),
                       ITpwdMailStatusText = string.Join(",", x.Select(g => g.ITpwdMailStatus==null?"": "Sent on " + g.ITpwdMailStatus?.ToString("dd-MM-yyyy"))),
                       CoOwnerITpwdMailStatusText = string.Join(",", x.Select(g => g.CoOwnerITpwdMailStatus == null ? "" : "Sent on "+ g.CoOwnerITpwdMailStatus?.ToString("dd-MM-yyyy"))),
                   }).ToList();

              
                try
                {
                    var withoutProp = _context.ViewCustomerWithoutProperty.ToList().Select(x => new ViewCustomerReportModel
                    {
                        CustomerName = x.CustomerName,
                        PAN = x.PAN,
                        CustomerID = x.CustomerID,
                        IsPanVerified = x.IsPanVerified,
                        Remarks =  formatRemarksWithoutProperty(x),
                        CustomerStatus=x.CustomerStatus                       
                    }).ToList();
                    if (!string.IsNullOrEmpty(filter.CustomerName)) {
                        withoutProp = withoutProp.AsQueryable().Where(x => x.CustomerName == filter.CustomerName).ToList();
                    }
                    if (!string.IsNullOrEmpty(filter.PAN))
                    {
                        withoutProp = withoutProp.AsQueryable().Where(x => x.CustomerName == filter.CustomerName).ToList();
                    }

                    final.AddRange(withoutProp);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

               return  final;
              
            }

            private string GetRemarks(List<ViewCustomerReport> list) {

                var remarks = "";
                var invalidPanInx = list.FindIndex(x =>( x.CustomerStatus==null?"": x.CustomerStatus).Contains("Invalid PAN"));
                for (var i = 0; i < list.Count; i++)
                {
                    var str = "";
                    if (invalidPanInx > -1)
                    {
                        var status = list[i].CustomerStatus == null ? "" : list[i].CustomerStatus;
                        if (!status.Contains("Invalid PAN"))
                            str = "Other owner Invalid PAN | " + formatRemarks(list[i]);
                        else
                            str = formatRemarks(list[i]);

                        if (i == list.Count - 1) 
                            remarks += str;
                        else
                            remarks += str+" , ";
                    }
                    else
                    {
                        str = formatRemarks(list[i]);
                        if (i == list.Count - 1)
                            remarks += str;
                        else
                            remarks += str + " , ";
                    }
                }
                return remarks;
            }


            private string formatRemarks(ViewCustomerReport model) {
                var remark = model.Remarks;
                if (!string.IsNullOrEmpty(remark)) {
                    remark += " | ";
                }
                if (model.InvalidPanDate != null)
                {
                   
                    remark += "Only TDS date : " + model.InvalidPanDate.Value.ToString("dd-MMM-yyy") +" | "+model.InvalidPanRemarks;
                }
                if (model.CustomerOptingOutDate != null)
                {
                    remark += "OptingOut date : " + model.CustomerOptingOutDate.Value.ToString("dd-MMM-yyy") + " | " + model.CustomerOptingOutRemarks;
                }
                return remark;
            
            }

            //Note customer flogs are moved to customer property so its not use anymore
            private string formatRemarksWithoutProperty(ViewCustomerWithoutProperty model)
            {
                var remark = "";
                if (model.InvalidPanDate != null)
                {
                    remark += "Only TDS date : " + model.InvalidPanDate.Value.ToString("dd-MMM-yyy") + " | " + model.InvalidPanRemarks;
                }
                if (model.CustomerOptingOutDate != null)
                {
                    remark += "OptingOut date : " + model.CustomerOptingOutDate.Value.ToString("dd-MMM-yyy") + " | " + model.CustomerOptingOutRemarks;
                }
                return remark;

            }
        }
    }
}
