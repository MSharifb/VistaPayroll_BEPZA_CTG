using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PGM.Web.Utility
{
    public enum CommonMessage
    {
        DeleteFailed, DeleteSuccessful, UpdateFailed, UpdateSuccessful, InsertFailed, InsertSuccessful, DataNotFound,MandatoryInputFailed,
        IDNotFound
    }
    public enum CommonAction
    {
        Save, Update, Delete, General
    }
    public enum ApplicationModule
    {  
        PRM = 1,
        PIM = 2,
        CPF = 7,
        ADC = 9,
        INV = 15,
        PMI = 16,
        AMS = 20
    }
}