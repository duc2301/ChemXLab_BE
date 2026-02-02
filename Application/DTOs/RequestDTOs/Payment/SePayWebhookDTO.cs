using Application.DTOs.Converter;
using System;
using System.Text.Json.Serialization;

namespace Application.DTOs.RequestDTOs.Payment
{
    /// <summary>
    /// DTO nhận Webhook/IPN từ SePay
    /// </summary>
    public class SePayWebhookDTO
    {
        /// <summary>
        /// ID giao dịch trên hệ thống SePay
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ngân hàng (VCB, ACB, MBBank, ...)
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Thời gian giao dịch
        /// </summary>
        [JsonConverter(typeof(SePayDateTimeConverter))]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Số tài khoản của bạn
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Tài khoản phụ (nếu có)
        /// </summary>
        public string? SubAccount { get; set; }

        /// <summary>
        /// Loại giao dịch: "in" (tiền vào) hoặc "out" (tiền ra)
        /// </summary>
        public string TransferType { get; set; }

        /// <summary>
        /// Số tiền giao dịch (quan trọng)
        /// </summary>
        public decimal TransferAmount { get; set; }

        /// <summary>
        /// Số dư lũy kế
        /// </summary>
        public decimal Accumulated { get; set; }

        /// <summary>
        /// Mã code thanh toán (nếu dùng)
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Nội dung chuyển khoản (quan trọng nhất)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Mã tham chiếu ngân hàng
        /// </summary>
        public string ReferenceCode { get; set; }

        /// <summary>
        /// Mô tả đầy đủ từ ngân hàng
        /// </summary>
        public string Description { get; set; }
    }
}
