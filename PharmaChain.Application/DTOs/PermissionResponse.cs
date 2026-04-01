using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaChain.Application.DTOs
{
    public class PermissionResponse
    {
        public int PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string? Module {  get; set; }
        public bool IsActive { get; set; }
    }
}
