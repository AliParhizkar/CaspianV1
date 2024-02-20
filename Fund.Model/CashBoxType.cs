using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    public class CashBoxType
    {
        public int Id { get; set; }

        public long Code { get; set; }

        public virtual string Name { get; set; }

        public string InternationalName { get; set; }

        public Currency Currency { get; set; }

        //public ValuableItem ValuableItem { get; set; }

        //public tellerTypeEnum TellerType { get; set; }

        //public situtationOperationTypeEnum Situation4Period { get; set; }

        //public situtationOperationTypeEnum Situation4Transaction { get; set; }

        public bool NoteSerialsRequired { get; set; }

        public bool NoteSerialIncludingCheckDigit { get; set; }

        public bool NotesIsBlockable { get; set; }

        public bool ControlingNotesValidity { get; set; }

        public bool NotesIsResalable { get; set; }

        public bool SellerBuyerInfoIsRequired { get; set; }

        public bool Exchangable { get; set; }

        public string Description { get; set; }

        //public atmTypeEnum AtmType { get; set; }

        //public ObservableCollection<CashBranch> ValidBranchs { get; set; }

        //public ObservableCollection<CashUserGroup> ValidUserGroups { get; set; }
    }
}