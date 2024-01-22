using AutoMapper;
using System.Collections.Generic;
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
    public static class PGMMappingExtension
    {

        //Salary Head Group
        public static SalaryHeadGroupViewModel ToModel(this PRM_SalaryHeadGroup obj)
        {
            return Mapper.Map<PRM_SalaryHeadGroup, SalaryHeadGroupViewModel>(obj);
        }
        public static PRM_SalaryHeadGroup ToEntity(this SalaryHeadGroupViewModel obj)
        {
            return Mapper.Map<SalaryHeadGroupViewModel, PRM_SalaryHeadGroup>(obj);
        }

        //Salary Head
        public static SalaryHeadViewModel ToModel(this PRM_SalaryHead obj)
        {
            return Mapper.Map<PRM_SalaryHead, SalaryHeadViewModel>(obj);
        }
        public static PRM_SalaryHead ToEntity(this SalaryHeadViewModel obj)
        {
            return Mapper.Map<SalaryHeadViewModel, PRM_SalaryHead>(obj);
        }

        //Salary Structure
        public static SalaryStructureModel ToModel(this PRM_SalaryStructure obj)
        {
            return Mapper.Map<PRM_SalaryStructure, SalaryStructureModel>(obj);
        }
        public static PRM_SalaryStructure ToEntity(this SalaryStructureModel obj)
        {
            return Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(obj);
        }

        public static List<PRM_SalaryStructure> ToEntityList(this List<SalaryStructureModel> modellist)
        {
            List<PRM_SalaryStructure> list = new List<PRM_SalaryStructure>();
            foreach (var item in modellist)
            {
                list.Add(Mapper.Map<SalaryStructureModel, PRM_SalaryStructure>(item));
            }
            return list;
        }

        //Salary Structure Details
        public static SalaryStructureDetailsModel ToModel(this PRM_SalaryStructureDetail obj)
        {
            return Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>(obj);
        }
        public static PRM_SalaryStructureDetail ToEntity(this SalaryStructureDetailsModel obj)
        {
            return Mapper.Map<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>(obj);
        }

        public static List<SalaryStructureDetailsModel> ToModelList(this List<PRM_SalaryStructureDetail> objlist)
        {
            List<SalaryStructureDetailsModel> list = new List<SalaryStructureDetailsModel>();
            foreach (var item in objlist)
            {
                list.Add(Mapper.Map<PRM_SalaryStructureDetail, SalaryStructureDetailsModel>(item));
            }

            return list;
        }
        public static List<PRM_SalaryStructureDetail> ToEntityList(this List<SalaryStructureDetailsModel> modellist)
        {
            List<PRM_SalaryStructureDetail> list = new List<PRM_SalaryStructureDetail>();
            foreach (var item in modellist)
            {
                list.Add(Mapper.Map<SalaryStructureDetailsModel, PRM_SalaryStructureDetail>(item));
            }

            return list;
        }

        //Employee Dalary Details
        public static SalaryStructureDetailsModel ToModel(this PRM_EmpSalaryDetail obj)
        {
            return Mapper.Map<PRM_EmpSalaryDetail, SalaryStructureDetailsModel>(obj);
        }

        //Bank Letter Body Text Mapping Extension
        public static BankAdviceLetterBodyViewModel ToModel(this PGM_BankAdviceLetterSubjectBody entity)
        {
            return Mapper.Map<PGM_BankAdviceLetterSubjectBody, BankAdviceLetterBodyViewModel>(entity);
        }
        public static PGM_BankAdviceLetterSubjectBody ToEntity(this BankAdviceLetterBodyViewModel model)
        {
            return Mapper.Map<BankAdviceLetterBodyViewModel, PGM_BankAdviceLetterSubjectBody>(model);
        }

        //Bank Letter Body variables Mapping Extension
        public static BankAdviceLetterBodyVariable ToModel(this PGM_BankAdviceLetterVariables entity)
        {
            return Mapper.Map<PGM_BankAdviceLetterVariables, BankAdviceLetterBodyVariable>(entity);
        }
        public static PGM_BankAdviceLetterVariables ToEntity(this BankAdviceLetterBodyVariable model)
        {
            return Mapper.Map<BankAdviceLetterBodyVariable, PGM_BankAdviceLetterVariables>(model);
        }

        //Bank Letter Template Setup Mapping Extension
        public static BankAdviceLetterTemplateViewModel ToModel(this PGM_BankAdviceLetterTemplate entity)
        {
            return Mapper.Map<PGM_BankAdviceLetterTemplate, BankAdviceLetterTemplateViewModel>(entity);
        }
        public static PGM_BankAdviceLetterTemplate ToEntity(this BankAdviceLetterTemplateViewModel model)
        {
            return Mapper.Map<BankAdviceLetterTemplateViewModel, PGM_BankAdviceLetterTemplate>(model);
        }

        //Salary Withheld Other than Time Sheet Issue [Before Generating Salary]
        public static SalaryWithheldModel ToModel(this PGM_SalaryWithheld entity)
        {
            return Mapper.Map<PGM_SalaryWithheld, SalaryWithheldModel>(entity);
        }
        public static PGM_SalaryWithheld ToEntity(this SalaryWithheldModel model)
        {
            return Mapper.Map<SalaryWithheldModel, PGM_SalaryWithheld>(model);
        }

        //Other Adjustment & Deduction
        public static OtherAdjustmentModel ToModel(this PGM_OtherAdjustDeduct entity)
        {
            return Mapper.Map<PGM_OtherAdjustDeduct, OtherAdjustmentModel>(entity);
        }
        public static PGM_OtherAdjustDeduct ToEntity(this OtherAdjustmentModel model)
        {
            return Mapper.Map<OtherAdjustmentModel, PGM_OtherAdjustDeduct>(model);
        }

        //Other Adjustment Style One
        public static OtherAdjustmentStyleOneModel ToStyleOneModel(this PGM_OtherAdjustDeduct entity)
        {
            return Mapper.Map<PGM_OtherAdjustDeduct, OtherAdjustmentStyleOneModel>(entity);
        }
        public static PGM_OtherAdjustDeduct ToStyleOneEntity(this OtherAdjustmentStyleOneModel model)
        {
            return Mapper.Map<OtherAdjustmentStyleOneModel, PGM_OtherAdjustDeduct>(model);
        }

        //Other Adjustment & Deduction All Mapping Extension
        public static OtherAdjustmentDeductAll ToModel(this PGM_OtherAdjustDeductAll entity)
        {
            return Mapper.Map<PGM_OtherAdjustDeductAll, OtherAdjustmentDeductAll>(entity);
        }
        public static PGM_OtherAdjustDeductAll ToEntity(this OtherAdjustmentDeductAll model)
        {
            return Mapper.Map<OtherAdjustmentDeductAll, PGM_OtherAdjustDeductAll>(model);
        }

        //Taxable Opening Income
        public static TaxOpeningDetailModel ToModel(this PGM_TaxOpeningDetail entity)
        {
            return Mapper.Map<PGM_TaxOpeningDetail, TaxOpeningDetailModel>(entity);
        }
        public static PGM_TaxOpeningDetail ToEntity(this TaxOpeningDetailModel model)
        {
            return Mapper.Map<TaxOpeningDetailModel, PGM_TaxOpeningDetail>(model);
        }

        public static TaxOpeningModel ToModel(this PGM_TaxOpening entity)
        {
            return Mapper.Map<PGM_TaxOpening, TaxOpeningModel>(entity);
        }
        public static PGM_TaxOpening ToEntity(this TaxOpeningModel model)
        {
            return Mapper.Map<TaxOpeningModel, PGM_TaxOpening>(model);
        }

        //Tax Rate
        public static TaxRateDetailModel ToModel(this PGM_TaxRateRuleDetail entity)
        {
            return Mapper.Map<PGM_TaxRateRuleDetail, TaxRateDetailModel>(entity);
        }
        public static PGM_TaxRateRuleDetail ToEntity(this TaxRateDetailModel model)
        {
            return Mapper.Map<TaxRateDetailModel, PGM_TaxRateRuleDetail>(model);
        }

        public static TaxRateModel ToModel(this PGM_TaxRateRule entity)
        {
            return Mapper.Map<PGM_TaxRateRule, TaxRateModel>(entity);
        }
        public static PGM_TaxRateRule ToEntity(this TaxRateModel model)
        {
            return Mapper.Map<TaxRateModel, PGM_TaxRateRule>(model);
        }

        //Tax Rule
        public static TaxRuleModel ToModel(this PGM_TaxRule entity)
        {
            return Mapper.Map<PGM_TaxRule, TaxRuleModel>(entity);
        }
        public static PGM_TaxRule ToEntity(this TaxRuleModel model)
        {
            return Mapper.Map<TaxRuleModel, PGM_TaxRule>(model);
        }

        //Bonus Type
        public static BonusTypeViewModel ToModel(this PGM_BonusType entity)
        {
            return Mapper.Map<PGM_BonusType, BonusTypeViewModel>(entity);
        }
        public static PGM_BonusType ToEntity(this BonusTypeViewModel viewModel)
        {
            return Mapper.Map<BonusTypeViewModel, PGM_BonusType>(viewModel);
        }

        //Bank Account
        public static BankAccountModel ToModel(this PGM_BankAccount entity)
        {
            return Mapper.Map<PGM_BankAccount, BankAccountModel>(entity);
        }
        public static PGM_BankAccount ToEntity(this BankAccountModel model)
        {
            return Mapper.Map<BankAccountModel, PGM_BankAccount>(model);
        }

        //Gratuity Rule
        public static GratuityRuleModel ToModel(this PGM_GratuityRule entity)
        {
            return Mapper.Map<PGM_GratuityRule, GratuityRuleModel>(entity);
        }
        public static PGM_GratuityRule ToEntity(this GratuityRuleModel model)
        {
            return Mapper.Map<GratuityRuleModel, PGM_GratuityRule>(model);
        }      

        // Leave Encashment mapping extension
        public static LeaveEncashmentViewModel ToModel(this PGM_LeaveEncashment entity)
        {
            return Mapper.Map<PGM_LeaveEncashment, LeaveEncashmentViewModel>(entity);
        }

        public static PGM_LeaveEncashment ToEntity(this LeaveEncashmentViewModel model)
        {
            return Mapper.Map<LeaveEncashmentViewModel, PGM_LeaveEncashment>(model);
        }


        // Bank Advice Letter mapping extension
        public static BankAdviceLetterViewModel ToModel(this PGM_BankLetter entity)
        {
            return Mapper.Map<PGM_BankLetter, BankAdviceLetterViewModel>(entity);
        }

        public static PGM_BankLetter ToEntity(this BankAdviceLetterViewModel model)
        {
            return Mapper.Map<BankAdviceLetterViewModel, PGM_BankLetter>(model);
        }

        public static BankAdviceLetterDetailModel ToModel(this PGM_BankLetterDetail entity)
        {
            return Mapper.Map<PGM_BankLetterDetail, BankAdviceLetterDetailModel>(entity);
        }
        public static PGM_BankLetterDetail ToEntity(this BankAdviceLetterDetailModel model)
        {
            return Mapper.Map<BankAdviceLetterDetailModel, PGM_BankLetterDetail>(model);
        }

        //Withheld Salary Payment Mapping Extension
        public static WithheldSalaryPaymentViewModel ToModel(this PGM_WithheldSalaryPayment entity)
        {
            return Mapper.Map<PGM_WithheldSalaryPayment, WithheldSalaryPaymentViewModel>(entity);
        }

        public static PGM_WithheldSalaryPayment ToEntity(this WithheldSalaryPaymentViewModel model)
        {
            return Mapper.Map<WithheldSalaryPaymentViewModel, PGM_WithheldSalaryPayment>(model);
        }

        // Gratuity Settlement Mapping Extension

        public static GratuitySettlementViewModel ToModel(this PGM_GratuityPayment entity)
        {
            return Mapper.Map<PGM_GratuityPayment, GratuitySettlementViewModel>(entity);
        }

        public static PGM_GratuityPayment ToEntity(this GratuitySettlementViewModel model)
        {
            return Mapper.Map<GratuitySettlementViewModel, PGM_GratuityPayment>(model);
        }

        // Final Settlement Mapping Extension

        public static FinalSettlementViewModel ToModel(this PGM_FinalSettlement entity)
        {
            return Mapper.Map<PGM_FinalSettlement, FinalSettlementViewModel>(entity);
        }

        public static PGM_FinalSettlement ToEntity(this FinalSettlementViewModel model)
        {
            return Mapper.Map<FinalSettlementViewModel, PGM_FinalSettlement>(model);
        }

        //Vehicle Deduction Bill Mapping Extension
        public static VehicleDeductionBillViewModel ToModel(this PGM_VehicleDeduction entity)
        {
            return Mapper.Map<PGM_VehicleDeduction, VehicleDeductionBillViewModel>(entity);
        }
        public static PGM_VehicleDeduction ToEntity(this VehicleDeductionBillViewModel model)
        {
            return Mapper.Map<VehicleDeductionBillViewModel, PGM_VehicleDeduction>(model);
        }

        // Bonus Details Mapping Extension
        public static BonusDetailsViewModel ToModel(this PGM_BonusDetail entity)
        {
            return Mapper.Map<PGM_BonusDetail, BonusDetailsViewModel>(entity);
        }

        public static PGM_BonusDetail ToEntity(this BonusDetailsViewModel model)
        {
            return Mapper.Map<BonusDetailsViewModel, PGM_BonusDetail>(model);
        }

        // House Rent Rule
        public static HouseRentRuleModel ToModel(this PGM_HouseRentRule entity)
        {
            return Mapper.Map<PGM_HouseRentRule, HouseRentRuleModel>(entity);
        }
        public static PGM_HouseRentRule ToEntity(this HouseRentRuleModel model)
        {
            return Mapper.Map<HouseRentRuleModel, PGM_HouseRentRule>(model);
        }

        // House Rent Rule Detail
        public static HouseRentRuleDetailModel ToModel(this PGM_HouseRentRuleDetail entity)
        {
            return Mapper.Map<PGM_HouseRentRuleDetail, HouseRentRuleDetailModel>(entity);
        }
        public static PGM_HouseRentRuleDetail ToEntity(this HouseRentRuleDetailModel model)
        {
            return Mapper.Map<HouseRentRuleDetailModel, PGM_HouseRentRuleDetail>(model);
        }

        // House Maintenance Charge
        public static HouseMaintenanceChargeViewModel ToModel(this PGM_HouseMaintenanceCharge entity)
        {
            return Mapper.Map<PGM_HouseMaintenanceCharge, HouseMaintenanceChargeViewModel>(entity);
        }
        public static PGM_HouseMaintenanceCharge ToEntity(this HouseMaintenanceChargeViewModel model)
        {
            return Mapper.Map<HouseMaintenanceChargeViewModel, PGM_HouseMaintenanceCharge>(model);
        }

        public static HouseMaintenanceChargeDetailViewModel ToModel(this PGM_HouseMaintenanceChargeDetail entity)
        {
            return Mapper.Map<PGM_HouseMaintenanceChargeDetail, HouseMaintenanceChargeDetailViewModel>(entity);
        }
        public static PGM_HouseMaintenanceChargeDetail ToEntity(this HouseMaintenanceChargeDetailViewModel model)
        {
            return Mapper.Map<HouseMaintenanceChargeDetailViewModel, PGM_HouseMaintenanceChargeDetail>(model);
        }

        // Electric Bill 
        public static ElectricBillViewModel ToModel(this PGM_ElectricBill entity)
        {
            return Mapper.Map<PGM_ElectricBill, ElectricBillViewModel>(entity);
        }
        public static PGM_ElectricBill ToEntity(this ElectricBillViewModel model)
        {
            return Mapper.Map<ElectricBillViewModel, PGM_ElectricBill>(model);
        }

        // Night Bill Payment 
        public static NightBillPaymentViewModel ToModel(this PGM_NightBillPayment entity)
        {
            return Mapper.Map<PGM_NightBillPayment, NightBillPaymentViewModel>(entity);
        }
        public static PGM_NightBillPayment ToEntity(this NightBillPaymentViewModel model)
        {
            return Mapper.Map<NightBillPaymentViewModel, PGM_NightBillPayment>(model);
        }
        public static NightBillPaymentDetailViewModel ToModel(this PGM_NightBillPaymentDetail entity)
        {
            return Mapper.Map<PGM_NightBillPaymentDetail, NightBillPaymentDetailViewModel>(entity);
        }
        public static PGM_NightBillPaymentDetail ToEntity(this NightBillPaymentDetailViewModel model)
        {
            return Mapper.Map<NightBillPaymentDetailViewModel, PGM_NightBillPaymentDetail>(model);
        }

        //Charge Alloowance Rule
        public static ChargeAllowanceRuleModel ToModel(this PGM_ChargeAllowanceRule entity)
        {
            return Mapper.Map<PGM_ChargeAllowanceRule, ChargeAllowanceRuleModel>(entity);
        }
        public static PGM_ChargeAllowanceRule ToEntity(this ChargeAllowanceRuleModel model)
        {
            return Mapper.Map<ChargeAllowanceRuleModel, PGM_ChargeAllowanceRule>(model);
        } 
        // Group Insurance Payment Type
        public static GroupInsurancePaymentTypeViewModel ToModel(this PGM_GroupInsurancePaymentType entity)
        {
            return Mapper.Map<PGM_GroupInsurancePaymentType, GroupInsurancePaymentTypeViewModel>(entity);
        }
        public static PGM_GroupInsurancePaymentType ToEntity(this GroupInsurancePaymentTypeViewModel model)
        {
            return Mapper.Map<GroupInsurancePaymentTypeViewModel, PGM_GroupInsurancePaymentType>(model);
        }

        // EmpChargeAllowance
        public static EmpChargeAllowanceModel ToModel(this PGM_EmpChargeAllowance entity)
        {
            return Mapper.Map<PGM_EmpChargeAllowance, EmpChargeAllowanceModel>(entity);
        }
        public static PGM_EmpChargeAllowance ToEntity(this EmpChargeAllowanceModel model)
        {
            return Mapper.Map<EmpChargeAllowanceModel, PGM_EmpChargeAllowance>(model);
        }

        // Group Insurance Payment
        public static GroupInsurancePaymentViewModel ToModel(this PGM_GroupInsurancePayment entity)
        {
            return Mapper.Map<PGM_GroupInsurancePayment, GroupInsurancePaymentViewModel>(entity);
        }
        public static PGM_GroupInsurancePayment ToEntity(this GroupInsurancePaymentViewModel model)
        {
            return Mapper.Map<GroupInsurancePaymentViewModel, PGM_GroupInsurancePayment>(model);
        }

        // House Rent Wate rBill Deduct
        public static HouseRentWaterBillDeductModel ToModel(this PGM_HouseRentWaterBillDeduct entity)
        {
            return Mapper.Map<PGM_HouseRentWaterBillDeduct, HouseRentWaterBillDeductModel>(entity);
        }
        public static PGM_HouseRentWaterBillDeduct ToEntity(this HouseRentWaterBillDeductModel model)
        {
            return Mapper.Map<HouseRentWaterBillDeductModel, PGM_HouseRentWaterBillDeduct>(model);
        }


        // Arrear Adjustment 
        public static ArrearAdjustmentModel ToModel(this PGM_ArrearAdjustment entity)
        {
            return Mapper.Map<PGM_ArrearAdjustment, ArrearAdjustmentModel>(entity);
        }
        public static PGM_ArrearAdjustment ToEntity(this ArrearAdjustmentModel model)
        {
            return Mapper.Map<ArrearAdjustmentModel, PGM_ArrearAdjustment>(model);
        }

        // Arrear Adjustment Detail
        public static ArrearAdjustmentDetailModel ToModel(this PGM_ArrearAdjustmentDetail entity)
        {
            return Mapper.Map<PGM_ArrearAdjustmentDetail, ArrearAdjustmentDetailModel>(entity);
        }
        public static PGM_ArrearAdjustmentDetail ToEntity(this ArrearAdjustmentDetailModel model)
        {
            return Mapper.Map<ArrearAdjustmentDetailModel, PGM_ArrearAdjustmentDetail>(model);
        }

        // Overtime
        public static OvertimeModel ToModel(this PGM_Overtime entity)
        {
            return Mapper.Map<PGM_Overtime, OvertimeModel>(entity);
        }
        public static PGM_Overtime ToEntity(this OvertimeModel model)
        {
            return Mapper.Map<OvertimeModel, PGM_Overtime>(model);
        }

        //Tax Region Rule
        public static TaxRegionRuleViewModel ToModel(this PGM_TaxRegionRule entity)
        {
            return Mapper.Map<PGM_TaxRegionRule, TaxRegionRuleViewModel>(entity);
        }
        public static PGM_TaxRegionRule ToEntity(this TaxRegionRuleViewModel model)
        {
            return Mapper.Map<TaxRegionRuleViewModel, PGM_TaxRegionRule>(model);
        }

        //Region wise Minimum Tax
        public static RegionWiseMinTaxViewModel ToModel(this PGM_TaxRegionWiseMinTax entity)
        {
            return Mapper.Map<PGM_TaxRegionWiseMinTax, RegionWiseMinTaxViewModel>(entity);
        }
        public static PGM_TaxRegionWiseMinTax ToEntity(this RegionWiseMinTaxViewModel model)
        {
            return Mapper.Map<RegionWiseMinTaxViewModel, PGM_TaxRegionWiseMinTax>(model);
        }

        //Advance Tax Paid
        public static AdvancedTaxPaidViewModel ToModel(this PGM_TaxOtherInvestAndAdvPaid entity)
        {
            return Mapper.Map<PGM_TaxOtherInvestAndAdvPaid, AdvancedTaxPaidViewModel>(entity);
        }
        public static PGM_TaxOtherInvestAndAdvPaid ToEntity(this AdvancedTaxPaidViewModel model)
        {
            return Mapper.Map<AdvancedTaxPaidViewModel, PGM_TaxOtherInvestAndAdvPaid>(model);
        }


        //Advance Tax Paid Detail
        public static AdvancedTaxPaidDetailViewModel ToModel(this PGM_TaxOtherInvestAndAdvPaidDetail entity)
        {
            return Mapper.Map<PGM_TaxOtherInvestAndAdvPaidDetail, AdvancedTaxPaidDetailViewModel>(entity);
        }
        public static PGM_TaxOtherInvestAndAdvPaidDetail ToEntity(this AdvancedTaxPaidDetailViewModel model)
        {
            return Mapper.Map<AdvancedTaxPaidDetailViewModel, PGM_TaxOtherInvestAndAdvPaidDetail>(model);
        }

        //Other Investment
        public static TaxOtherInvestmentViewModel ToInvestmentModel(this PGM_TaxOtherInvestAndAdvPaid entity)
        {
            return Mapper.Map<PGM_TaxOtherInvestAndAdvPaid, TaxOtherInvestmentViewModel>(entity);
        }
        public static PGM_TaxOtherInvestAndAdvPaid ToInvestmentEntity(this TaxOtherInvestmentViewModel model)
        {
            return Mapper.Map<TaxOtherInvestmentViewModel, PGM_TaxOtherInvestAndAdvPaid>(model);
        }

        //Other Investment Detail
        public static TaxOtherInvestmentDetailViewModel ToInvestmentModel(this PGM_TaxOtherInvestAndAdvPaidDetail entity)
        {
            return Mapper.Map<PGM_TaxOtherInvestAndAdvPaidDetail, TaxOtherInvestmentDetailViewModel>(entity);
        }
        public static PGM_TaxOtherInvestAndAdvPaidDetail ToInvestmentEntity(this TaxOtherInvestmentDetailViewModel model)
        {
            return Mapper.Map<TaxOtherInvestmentDetailViewModel, PGM_TaxOtherInvestAndAdvPaidDetail>(model);
        }

        //Document
        public static DocumentViewModel ToModel(this PGM_Document entity)
        {
            return Mapper.Map<PGM_Document, DocumentViewModel>(entity);
        }
        public static PGM_Document ToEntity(this DocumentViewModel model)
        {
            return Mapper.Map<DocumentViewModel, PGM_Document>(model);
        }

        //Other Investment Type
        public static InvestmentTypeViewModel ToModel(this PGM_TaxOtherInvestmentType entity)
        {
            return Mapper.Map<PGM_TaxOtherInvestmentType, InvestmentTypeViewModel>(entity);
        }
        public static PGM_TaxOtherInvestmentType ToEntity(this InvestmentTypeViewModel model)
        {
            return Mapper.Map<InvestmentTypeViewModel, PGM_TaxOtherInvestmentType>(model);
        }

        //Tax Exemption Rule
        public static TaxExemptionRuleViewModel ToModel(this PGM_TaxExemptionRule entity)
        {
            return Mapper.Map<PGM_TaxExemptionRule, TaxExemptionRuleViewModel>(entity);
        }
        public static PGM_TaxExemptionRule ToEntity(this TaxExemptionRuleViewModel model)
        {
            return Mapper.Map<TaxExemptionRuleViewModel, PGM_TaxExemptionRule>(model);
        }
        //Tax Exemption Rule Detail
        public static TaxExemptionRuleDetailViewModel ToModel(this PGM_TaxExemptionRuleDetail entity)
        {
            return Mapper.Map<PGM_TaxExemptionRuleDetail, TaxExemptionRuleDetailViewModel>(entity);
        }
        public static PGM_TaxExemptionRuleDetail ToEntity(this TaxExemptionRuleDetailViewModel model)
        {
            return Mapper.Map<TaxExemptionRuleDetailViewModel, PGM_TaxExemptionRuleDetail>(model);
        }

        //Tax Investment Rebate Rule
        public static TaxInvestmentRebateRuleViewModel ToModel(this PGM_TaxInvestmentRebateRule entity)
        {
            return Mapper.Map<PGM_TaxInvestmentRebateRule, TaxInvestmentRebateRuleViewModel>(entity);
        }
        public static PGM_TaxInvestmentRebateRule ToEntity(this TaxInvestmentRebateRuleViewModel model)
        {
            return Mapper.Map<TaxInvestmentRebateRuleViewModel, PGM_TaxInvestmentRebateRule>(model);
        }

        //Tax Investment Rebate Rule Master
        public static TaxInvestmentRebateRuleMasterViewModel ToModel(this PGM_TaxInvestmentRebateRuleMaster entity)
        {
            return Mapper.Map<PGM_TaxInvestmentRebateRuleMaster, TaxInvestmentRebateRuleMasterViewModel>(entity);
        }
        public static PGM_TaxInvestmentRebateRuleMaster ToEntity(this TaxInvestmentRebateRuleMasterViewModel model)
        {
            return Mapper.Map<TaxInvestmentRebateRuleMasterViewModel, PGM_TaxInvestmentRebateRuleMaster>(model);
        }

        //Tax Investment Rebate Rule Detail
        public static TaxInvestmentRebateRuleDetailViewModel ToModel(this PGM_TaxInvestmentRebateRuleDetail entity)
        {
            return Mapper.Map<PGM_TaxInvestmentRebateRuleDetail, TaxInvestmentRebateRuleDetailViewModel>(entity);
        }
        public static PGM_TaxInvestmentRebateRuleDetail ToEntity(this TaxInvestmentRebateRuleDetailViewModel model)
        {
            return Mapper.Map<TaxInvestmentRebateRuleDetailViewModel, PGM_TaxInvestmentRebateRuleDetail>(model);
        }

        //Tax Withheld
        public static TaxWithheldViewModel ToModel(this PGM_TaxWithheld entity)
        {
            return Mapper.Map<PGM_TaxWithheld, TaxWithheldViewModel>(entity);
        }
        public static PGM_TaxWithheld ToEntity(this TaxWithheldViewModel model)
        {
            return Mapper.Map<TaxWithheldViewModel, PGM_TaxWithheld>(model);
        }

        //Import Attendance
        public static ImportAttendanceViewModel ToModel(this PGM_Attendance entity)
        {
            return Mapper.Map<PGM_Attendance, ImportAttendanceViewModel>(entity);
        }
        public static PGM_Attendance ToEntity(this ImportAttendanceViewModel model)
        {
            return Mapper.Map<ImportAttendanceViewModel, PGM_Attendance>(model);
        }

        //Import Refreshment
        public static ImportRefreshmentViewModel ToModel(this PGM_Refreshment entity)
        {
            return Mapper.Map<PGM_Refreshment, ImportRefreshmentViewModel>(entity);
        }
        public static PGM_Refreshment ToEntity(this ImportRefreshmentViewModel model)
        {
            return Mapper.Map<ImportRefreshmentViewModel, PGM_Refreshment>(model);
        }

        // Refreshment
        public static RefreshmentViewModel ToViewModel(this PGM_Refreshment entity)
        {
            return Mapper.Map<PGM_Refreshment, RefreshmentViewModel>(entity);
        }
        public static PGM_Refreshment ToEntity(this RefreshmentViewModel model)
        {
            return Mapper.Map<RefreshmentViewModel, PGM_Refreshment>(model);
        }

        // Attendance
        public static AttendanceViewModel ToViewModel(this PGM_Attendance entity)
        {
            return Mapper.Map<PGM_Attendance, AttendanceViewModel>(entity);
        }
        public static PGM_Attendance ToEntity(this AttendanceViewModel model)
        {
            return Mapper.Map<AttendanceViewModel, PGM_Attendance>(model);
        }

        // vwPGMEmploymentInfo
        public static EmployeeInfoModel ToModel(this vwPGMEmploymentInfo entity)
        {
            return Mapper.Map<vwPGMEmploymentInfo, EmployeeInfoModel>(entity);
        }

        // CustomPropertyAttribute
        public static CustomPropertiesModel ToModel(this CustomPropertyAttribute entity)
        {
            return Mapper.Map<CustomPropertyAttribute, CustomPropertiesModel>(entity);
        }
        public static CustomPropertyAttribute ToEntity(this CustomPropertiesModel model)
        {
            return Mapper.Map<CustomPropertiesModel, CustomPropertyAttribute>(model);
        }

        // CustomReportBackOffice
        public static CustomReportModel ToModel(this CustomReportBackOffice entity)
        {
            return Mapper.Map<CustomReportBackOffice, CustomReportModel>(entity);
        }
        public static CustomReportBackOffice ToEntity(this CustomReportModel model)
        {
            return Mapper.Map<CustomReportModel, CustomReportBackOffice>(model);
        }

        //Salary Head COA and SubLedger Mapping
        public static SalaryHeadCOASubledgerMappingViewModel ToModel(this PGM_SalaryHeadCOASubledgerMapping entity)
        {
            return Mapper.Map<PGM_SalaryHeadCOASubledgerMapping, SalaryHeadCOASubledgerMappingViewModel>(entity);
        }
        public static PGM_SalaryHeadCOASubledgerMapping ToEntity(this SalaryHeadCOASubledgerMappingViewModel model)
        {
            return Mapper.Map<SalaryHeadCOASubledgerMappingViewModel, PGM_SalaryHeadCOASubledgerMapping>(model);
        }
        //Exclude Emp From Report
        public static ExcludeEmpFromReportModel ToModel(this PGM_ExcludeEmpFromSalaryHead entity)
        {
            return Mapper.Map<PGM_ExcludeEmpFromSalaryHead, ExcludeEmpFromReportModel>(entity);
        }
        public static PGM_ExcludeEmpFromSalaryHead ToEntity(this ExcludeEmpFromReportModel model)
        {
            return Mapper.Map<ExcludeEmpFromReportModel, PGM_ExcludeEmpFromSalaryHead>(model);
        }
    }
}