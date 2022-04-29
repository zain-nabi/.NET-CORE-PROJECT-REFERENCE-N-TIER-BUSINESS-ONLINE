using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.CRM.Tables;

namespace Triton.Interface.CRM
{
    public interface IFinance
    {
        Task<long> Post(CRNoteTemp crNoteTemp);
    }
}
