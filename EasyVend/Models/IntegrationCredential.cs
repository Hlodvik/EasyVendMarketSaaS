using System;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class IntegrationCredential
    {
        [Key]
        public Guid CredentialId { get; set; }
        public Guid IntegrationId { get; set; }
        public string KeyName { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty; // TODO: encrypt
    }
}
