using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Sepay
{
    public class SePaySettings
    {
        public string MerchantId { get; set; }
        public string SecretKey { get; set; }
        public string Environment { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public string QrBaseUrl { get; set; }
    }
}
