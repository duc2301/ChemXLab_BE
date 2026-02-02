using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Sepay
{
    /// <summary>
    /// Configuration settings for SePay integration.
    /// </summary>
    public class SePaySettings
    {
        /// <summary>
        /// The merchant identifier provided by SePay.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The secret key used for authentication or signature verification.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// The environment setting (e.g., "sandbox" or "production").
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The bank code associated with the receiving account.
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// The bank account number for receiving payments.
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// The base URL used to generate QR codes.
        /// </summary>
        public string QrBaseUrl { get; set; }
    }
}