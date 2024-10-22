using System.ComponentModel;

namespace BusinessLayer.Model.Common.GlobalEnums
{
    public enum TransactionStateEnum
    {
        Empty = 0,
        Add = 1,
        Edit = 2,
        Editable = TransactionStateEnum.Add | TransactionStateEnum.Edit,
        View = 4,
        Delete = 8,
    }
}
