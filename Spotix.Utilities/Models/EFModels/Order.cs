﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Spotix.Utilities.Models.EFModels;

public partial class Order
{
    public int Id { get; set; }

    public DateTime CreatedTime { get; set; }

    public int Total { get; set; }

    public string Payment { get; set; }

    public string UserId { get; set; }

    public string Number { get; set; }

    public virtual ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();

    public virtual User User { get; set; }
}