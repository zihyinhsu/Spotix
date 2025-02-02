﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Spotix.Utilities.Models.EFModels;

public partial class User : IdentityUser
{
    public string LineId { get; set; }

    public bool Gender { get; set; }

	public DateTime Birthday { get; set; }

	public string Address { get; set; }

    public string AvatarUrl { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

	public List<string> Roles { get; set; }
}