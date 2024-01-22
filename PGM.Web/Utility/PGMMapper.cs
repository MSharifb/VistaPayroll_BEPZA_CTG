using AutoMapper;
using DAL.PGM;
using PGM.Web.Areas.PGM.Models.ArrearAdjustment;
using PGM.Web.Areas.PGM.Models.Attendance;
using PGM.Web.Areas.PGM.Models.BankAccount;
using PGM.Web.Areas.PGM.Models.BankAdviceLetter;
using PGM.Web.Areas.PGM.Models.BankAdviceLetterSubjectBody;
using PGM.Web.Areas.PGM.Models.BankAdviceLetterTemplate;
using PGM.Web.Areas.PGM.Models.BonusProcess;
using PGM.Web.Areas.PGM.Models.BonusType;
using PGM.Web.Areas.PGM.Models.ChargeAllowanceRule;
using PGM.Web.Areas.PGM.Models.CustomProperties;
using PGM.Web.Areas.PGM.Models.CustomReport;
using PGM.Web.Areas.PGM.Models.Document;
using PGM.Web.Areas.PGM.Models.ElectricBill;
using PGM.Web.Areas.PGM.Models.EmpChargeAllowance;
using PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure;
using PGM.Web.Areas.PGM.Models.FinalSettlement;
using PGM.Web.Areas.PGM.Models.GratuityRule;
using PGM.Web.Areas.PGM.Models.GratuitySettlement;
using PGM.Web.Areas.PGM.Models.GroupInsurancePayment;
using PGM.Web.Areas.PGM.Models.GroupInsurancePaymentType;
using PGM.Web.Areas.PGM.Models.HouseMaintenanceCharge;
using PGM.Web.Areas.PGM.Models.HouseRentRule;
using PGM.Web.Areas.PGM.Models.HouseRentWaterBillDeduct;
using PGM.Web.Areas.PGM.Models.ImportXl;
using PGM.Web.Areas.PGM.Models.LeaveEncashment;
using PGM.Web.Areas.PGM.Models.NightBill;
using PGM.Web.Areas.PGM.Models.OtherAdjustment;
using PGM.Web.Areas.PGM.Models.OtherAdjustmentStyleOne;
using PGM.Web.Areas.PGM.Models.OtherAdjustmentDeductionAll;
using PGM.Web.Areas.PGM.Models.OverTime;
using PGM.Web.Areas.PGM.Models.Refreshment;
using PGM.Web.Areas.PGM.Models.SalaryHead;
using PGM.Web.Areas.PGM.Models.SalaryHeadGroup;
using PGM.Web.Areas.PGM.Models.SalaryStructure;
using PGM.Web.Areas.PGM.Models.SalaryWithheld;
using PGM.Web.Areas.PGM.Models.TaxAdvancedPaid;
using PGM.Web.Areas.PGM.Models.TaxExemptionRule;
using PGM.Web.Areas.PGM.Models.TaxInvestmentRebateRule;
using PGM.Web.Areas.PGM.Models.TaxOpening;
using PGM.Web.Areas.PGM.Models.TaxOtherInvestment;
using PGM.Web.Areas.PGM.Models.TaxRate;
using PGM.Web.Areas.PGM.Models.TaxRegion;
using PGM.Web.Areas.PGM.Models.TaxRule;
using PGM.Web.Areas.PGM.Models.TaxWithheld;
using PGM.Web.Areas.PGM.Models.VehicleDeductionBill;
using PGM.Web.Areas.PGM.Models.WithheldSalaryPayment;
using PGM.Web.Areas.PGM.Models.SalaryHeadCOASubledgerMapping;
using PGM.Web.Areas.PGM.Models.ExcludeEmpFromReport;

namespace PGM.Web.Utility
{
    public class PGMMapper
    {
        public PGMMapper()
        {

            //Salary Head Group
            Mapper.CreateMap<SalaryHeadGroupViewModel, PRM_SalaryHeadGroup>();
            Mapper.CreateMap<PRM_SalaryHeadGroup, SalaryHeadGroupViewModel>();

            //Salary Head
            Mapper.CreateMap<SalaryHeadViewModel, PRM_SalaryHead>();
            Mapper.CreateMap<PRM_SalaryHead, SalaryHeadViewModel>();

            //Salary Structure
            Mapper.CreateMap<SalaryStructureModel, PRM_SalaryStructure>();
            Mapper.CreateMap<PRM_SalaryStructure, SalaryStructureModel>();

            //Salary Structure Details
            Mapper.CreateMap<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>();
            Mapper.CreateMap<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>();

            //emp salary structure
            Mapper.CreateMap<SalaryStructureDetailsModel, PRM_EmpSalaryDetail>();
            Mapper.CreateMap<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>();

            //Bank Letter Body
            Mapper.CreateMap<BankAdviceLetterBodyViewModel, PGM_BankAdviceLetterSubjectBody>();
            Mapper.CreateMap<PGM_BankAdviceLetterSubjectBody, BankAdviceLetterBodyViewModel>();

            //Bank Letter Body Variables
            Mapper.CreateMap<BankAdviceLetterBodyVariable, PGM_BankAdviceLetterVariables>();
            Mapper.CreateMap<PGM_BankAdviceLetterVariables, BankAdviceLetterBodyVariable>();

            //Bank Letter Template Setup
            Mapper.CreateMap<BankAdviceLetterTemplateViewModel, PGM_BankAdviceLetterTemplate>();
            Mapper.CreateMap<PGM_BankAdviceLetterTemplate, BankAdviceLetterTemplateViewModel>();

            //Withheld Salary
            Mapper.CreateMap<SalaryWithheldModel, PGM_SalaryWithheld>();
            Mapper.CreateMap<PGM_SalaryWithheld, SalaryWithheldModel>();


            //Other Adjustment and Deduction
            Mapper.CreateMap<OtherAdjustmentModel, PGM_OtherAdjustDeduct>();
            Mapper.CreateMap<PGM_OtherAdjustDeduct, OtherAdjustmentModel>();

            //Other Adjustment and Deduction Style One
            Mapper.CreateMap<OtherAdjustmentStyleOneModel, PGM_OtherAdjustDeduct>();
            Mapper.CreateMap<PGM_OtherAdjustDeduct, OtherAdjustmentStyleOneModel>();

            //Other Adjustment and Deduction All Mapper
            Mapper.CreateMap<OtherAdjustmentDeductAll, PGM_OtherAdjustDeductAll>();
            Mapper.CreateMap<PGM_OtherAdjustDeductAll, OtherAdjustmentDeductAll>();

            //Taxable Opening Income
            Mapper.CreateMap<TaxOpeningDetailModel, PGM_TaxOpeningDetail>();
            Mapper.CreateMap<PGM_TaxOpeningDetail, TaxOpeningDetailModel>();

            Mapper.CreateMap<TaxOpeningModel, PGM_TaxOpening>();
            Mapper.CreateMap<PGM_TaxOpening, TaxOpeningModel>();

            //Tax Rate
            Mapper.CreateMap<TaxRateDetailModel, PGM_TaxRateRuleDetail>();
            Mapper.CreateMap<PGM_TaxRateRuleDetail, TaxRateDetailModel>();

            Mapper.CreateMap<TaxRateModel, PGM_TaxRateRule>();
            Mapper.CreateMap<PGM_TaxRateRule, TaxRateModel>();

            //Tax Rules
            Mapper.CreateMap<TaxRuleModel, PGM_TaxRule>();
            Mapper.CreateMap<PGM_TaxRule, TaxRuleModel>();

            //Bonus Type
            Mapper.CreateMap<BonusTypeViewModel, PGM_BonusType>();
            Mapper.CreateMap<PGM_BonusType, BonusTypeViewModel>();

            //Bank Account
            Mapper.CreateMap<BankAccountModel, PGM_BankAccount>();
            Mapper.CreateMap<PGM_BankAccount, BankAccountModel>();


            //Gratuity Rule
            Mapper.CreateMap<GratuityRuleModel, PGM_GratuityRule>();
            Mapper.CreateMap<PGM_GratuityRule, GratuityRuleModel>();

            // Leave Encashment Mapper
            Mapper.CreateMap<LeaveEncashmentViewModel, PGM_LeaveEncashment>();
            Mapper.CreateMap<PGM_LeaveEncashment, LeaveEncashmentViewModel>();

            // Bank Advice Letter Mapper
            Mapper.CreateMap<BankAdviceLetterViewModel, PGM_BankLetter>();
            Mapper.CreateMap<PGM_BankLetter, BankAdviceLetterViewModel>();

            Mapper.CreateMap<BankAdviceLetterDetailModel, PGM_BankLetterDetail>();
            Mapper.CreateMap<PGM_BankLetterDetail, BankAdviceLetterDetailModel>();

            // Withheld Salary Paument Mapper
            Mapper.CreateMap<WithheldSalaryPaymentViewModel, PGM_WithheldSalaryPayment>();
            Mapper.CreateMap<PGM_WithheldSalaryPayment, WithheldSalaryPaymentViewModel>();

            // Gratuity Settlement Mapper
            Mapper.CreateMap<GratuitySettlementViewModel, PGM_GratuityPayment>();
            Mapper.CreateMap<PGM_GratuityPayment, GratuitySettlementViewModel>();

            // Final Settlement Mapper
            Mapper.CreateMap<FinalSettlementViewModel, PGM_FinalSettlement>();
            Mapper.CreateMap<PGM_FinalSettlement, FinalSettlementViewModel>();

            // Vehicle Deduction Mapper
            Mapper.CreateMap<VehicleDeductionBillViewModel, PGM_VehicleDeduction>();
            Mapper.CreateMap<PGM_VehicleDeduction, VehicleDeductionBillViewModel>();

            //Bonus Mapper
            Mapper.CreateMap<BonusDetailsViewModel, PGM_BonusDetail>();
            Mapper.CreateMap<PGM_BonusDetail, BonusDetailsViewModel>();

            //House Rent Rule
            Mapper.CreateMap<HouseRentRuleModel, PGM_HouseRentRule>();
            Mapper.CreateMap<PGM_HouseRentRule, HouseRentRuleModel>();

            //House Rent Rule Detail
            Mapper.CreateMap<HouseRentRuleDetailModel, PGM_HouseRentRuleDetail>();
            Mapper.CreateMap<PGM_HouseRentRuleDetail, HouseRentRuleDetailModel>();

            // House Maintenance Charge
            // Over Time Mapper
            Mapper.CreateMap<HouseMaintenanceChargeViewModel, PGM_HouseMaintenanceCharge>();
            Mapper.CreateMap<PGM_HouseMaintenanceCharge, HouseMaintenanceChargeViewModel>();

            Mapper.CreateMap<HouseMaintenanceChargeDetailViewModel, PGM_HouseMaintenanceChargeDetail>();
            Mapper.CreateMap<PGM_HouseMaintenanceChargeDetail, HouseMaintenanceChargeDetailViewModel>();

            // Electric Bill
            Mapper.CreateMap<ElectricBillViewModel, PGM_ElectricBill>();
            Mapper.CreateMap<PGM_ElectricBill, ElectricBillViewModel>();

            // Night Bill Payment 
            Mapper.CreateMap<NightBillPaymentViewModel, PGM_NightBillPayment>();
            Mapper.CreateMap<PGM_NightBillPayment, NightBillPaymentViewModel>();
            Mapper.CreateMap<NightBillPaymentDetailViewModel, PGM_NightBillPaymentDetail>();
            Mapper.CreateMap<PGM_NightBillPaymentDetail, NightBillPaymentDetailViewModel>();

            // Charge Allowance Rule
            Mapper.CreateMap<ChargeAllowanceRuleModel, PGM_ChargeAllowanceRule>();
            Mapper.CreateMap<PGM_ChargeAllowanceRule, ChargeAllowanceRuleModel>();

            // EmpChargeAllowance
            Mapper.CreateMap<EmpChargeAllowanceModel, PGM_EmpChargeAllowance>();
            Mapper.CreateMap<PGM_EmpChargeAllowance, EmpChargeAllowanceModel>();

            // Group Insurance Payment Type
            Mapper.CreateMap<GroupInsurancePaymentTypeViewModel, PGM_GroupInsurancePaymentType>();
            Mapper.CreateMap<PGM_GroupInsurancePaymentType, GroupInsurancePaymentTypeViewModel>();

            // Group Insurance Payment 
            Mapper.CreateMap<GroupInsurancePaymentViewModel, PGM_GroupInsurancePayment>();
            Mapper.CreateMap<PGM_GroupInsurancePayment, GroupInsurancePaymentViewModel>();

            // House Rent Wate rBill Deduct
            Mapper.CreateMap<HouseRentWaterBillDeductModel, PGM_HouseRentWaterBillDeduct>();
            Mapper.CreateMap<PGM_HouseRentWaterBillDeduct, HouseRentWaterBillDeductModel>();


            // Arrear Adjustment
            Mapper.CreateMap<ArrearAdjustmentModel, PGM_ArrearAdjustment>();
            Mapper.CreateMap<PGM_ArrearAdjustment, ArrearAdjustmentModel>();

            // Arrear Adjustment Detail
            Mapper.CreateMap<ArrearAdjustmentDetailModel, PGM_ArrearAdjustmentDetail>();
            Mapper.CreateMap<PGM_ArrearAdjustmentDetail, ArrearAdjustmentDetailModel>();

            // Overtime
            Mapper.CreateMap<OvertimeModel, PGM_Overtime>();
            Mapper.CreateMap<PGM_Overtime, OvertimeModel>();

            //TaxRegionRule
            Mapper.CreateMap<TaxRegionRuleViewModel, PGM_TaxRegionRule>();
            Mapper.CreateMap<PGM_TaxRegionRule, TaxRegionRuleViewModel>();

            //TaxRegionWiseMinTax
            Mapper.CreateMap<RegionWiseMinTaxViewModel, PGM_TaxRegionWiseMinTax>();
            Mapper.CreateMap<PGM_TaxRegionWiseMinTax, RegionWiseMinTaxViewModel>();

            //Advance Paid Tax
            Mapper.CreateMap<AdvancedTaxPaidViewModel, PGM_TaxOtherInvestAndAdvPaid>();
            Mapper.CreateMap<PGM_TaxOtherInvestAndAdvPaid, AdvancedTaxPaidViewModel>();

            //Advance Paid Tax Detail
            Mapper.CreateMap<AdvancedTaxPaidDetailViewModel, PGM_TaxOtherInvestAndAdvPaidDetail>();
            Mapper.CreateMap<PGM_TaxOtherInvestAndAdvPaidDetail, AdvancedTaxPaidDetailViewModel>();

            //Other Investment
            Mapper.CreateMap<TaxOtherInvestmentViewModel, PGM_TaxOtherInvestAndAdvPaid>();
            Mapper.CreateMap<PGM_TaxOtherInvestAndAdvPaid, TaxOtherInvestmentViewModel>();

            //Other Investment Detail
            Mapper.CreateMap<TaxOtherInvestmentDetailViewModel, PGM_TaxOtherInvestAndAdvPaidDetail>();
            Mapper.CreateMap<PGM_TaxOtherInvestAndAdvPaidDetail, TaxOtherInvestmentDetailViewModel>();

            //Document
            Mapper.CreateMap<DocumentViewModel, PGM_Document>();
            Mapper.CreateMap<PGM_Document, DocumentViewModel>();

            //Other Investment Type
            Mapper.CreateMap<InvestmentTypeViewModel, PGM_TaxOtherInvestmentType>();
            Mapper.CreateMap<PGM_TaxOtherInvestmentType, InvestmentTypeViewModel>();

            //Tax Exemption Rule
            Mapper.CreateMap<TaxExemptionRuleViewModel, PGM_TaxExemptionRule>();
            Mapper.CreateMap<PGM_TaxExemptionRule, TaxExemptionRuleViewModel>();

            //Tax Exemption Rule Detail
            Mapper.CreateMap<TaxExemptionRuleDetailViewModel, PGM_TaxExemptionRuleDetail>();
            Mapper.CreateMap<PGM_TaxExemptionRuleDetail, TaxExemptionRuleDetailViewModel>();

            //Tax Investment Rebate Rule
            Mapper.CreateMap<TaxInvestmentRebateRuleViewModel, PGM_TaxInvestmentRebateRule>();
            Mapper.CreateMap<PGM_TaxInvestmentRebateRule, TaxInvestmentRebateRuleViewModel>();

            //Tax Investment Rebate Rule Master
            Mapper.CreateMap<TaxInvestmentRebateRuleMasterViewModel, PGM_TaxInvestmentRebateRuleMaster>();
            Mapper.CreateMap<PGM_TaxInvestmentRebateRuleMaster, TaxInvestmentRebateRuleMasterViewModel>();

            //Tax Investment Rebate Rule Detail
            Mapper.CreateMap<TaxInvestmentRebateRuleDetailViewModel, PGM_TaxInvestmentRebateRuleDetail>();
            Mapper.CreateMap<PGM_TaxInvestmentRebateRuleDetail, TaxInvestmentRebateRuleDetailViewModel>();

            //Tax Withheld
            Mapper.CreateMap<TaxWithheldViewModel, PGM_TaxWithheld>();
            Mapper.CreateMap<PGM_TaxWithheld, TaxWithheldViewModel>();

            // Mapping Import Data - Attendance and Refreshment

            //Attendance
            Mapper.CreateMap<ImportAttendanceViewModel, PGM_Attendance>();
            Mapper.CreateMap<PGM_Attendance, ImportAttendanceViewModel>();

            //Refreshment
            Mapper.CreateMap<ImportRefreshmentViewModel, PGM_Refreshment>();
            Mapper.CreateMap<PGM_Refreshment, ImportRefreshmentViewModel>();

            //Refreshment ViewModel
            Mapper.CreateMap<RefreshmentViewModel, PGM_Refreshment>();
            Mapper.CreateMap<PGM_Refreshment, RefreshmentViewModel>();

            //Attendance ViewModel
            Mapper.CreateMap<AttendanceViewModel, PGM_Attendance>();
            Mapper.CreateMap<PGM_Attendance, AttendanceViewModel>();

            //Salary Head COA and SubLedger Mapping
            Mapper.CreateMap<SalaryHeadCOASubledgerMappingViewModel, PGM_SalaryHeadCOASubledgerMapping>();
            Mapper.CreateMap<PGM_SalaryHeadCOASubledgerMapping, SalaryHeadCOASubledgerMappingViewModel>();

            //Exclude Emp From Report
            Mapper.CreateMap<ExcludeEmpFromReportModel, PGM_ExcludeEmpFromSalaryHead>();
            Mapper.CreateMap<PGM_ExcludeEmpFromSalaryHead, ExcludeEmpFromReportModel>();

            // --------------------------------------------------------------

            // View - EmployeeInfoModel
            Mapper.CreateMap<EmployeeInfoModel, vwPGMEmploymentInfo>();
            Mapper.CreateMap<vwPGMEmploymentInfo, EmployeeInfoModel>();

            // View - CustomPropertiesModel
            Mapper.CreateMap<CustomPropertiesModel, CustomPropertyAttribute>();
            Mapper.CreateMap<CustomPropertyAttribute, CustomPropertiesModel>();

            // View - CustomReportModel
            Mapper.CreateMap<CustomReportModel, CustomReportBackOffice>();
            Mapper.CreateMap<CustomReportBackOffice, CustomReportModel>();
        }
    }
}
