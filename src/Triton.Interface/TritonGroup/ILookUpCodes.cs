using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Triton.Model.TritonGroup.Tables;

namespace Triton.Interface.TritonGroup
{
    public interface ILookUpCodes
    {
        Task<List<LookUpCodes>> GetTritonGroupLookUpCodesByLookupcodeIDs(string LookupcodeIDs);
    }
}
