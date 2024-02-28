using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Enums
{
    public enum OrderStatus : int
    {
        NewOrder = 1, // moi tao
        OrderReceived = 2, // tiep nhan
        OrderDelivered = 3,//da giao
        CustomerReceived = 4,// khac hang da nhan
        OrderCompleted = 5,// da nhan hang
        Cancelled = 6,// huy
    }

    public enum SesstionType : int
    {
        CONSULSTATION = 0,
        LAY_TEST = 1,
        HTS_POS = 2,
        PrEP = 3,
        ART = 4,
        ORDER_ITEM = 5,
    }

    public enum ReferType:int
    {
        TESTING,
        PrEP,
        ART
    }

    public enum OrderSort
    {
        DateCreate
    }

    public enum NotiBooking
    {
        ADD,
        CHANGE
    }



}
