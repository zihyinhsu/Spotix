﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Spotix.Utilities.Models.EFModels;

public partial class Event
{
    public int Id { get; set; }

    public string Name { get; set; }

    public byte[] Info { get; set; }

    public byte[] ImgUrl { get; set; }

    public int PlaceId { get; set; }

    public string Host { get; set; }

    public int DisplayOrder { get; set; }

    public virtual Place Place { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}