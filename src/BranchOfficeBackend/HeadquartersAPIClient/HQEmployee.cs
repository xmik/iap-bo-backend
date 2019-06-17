using System;
using Newtonsoft.Json;

namespace BranchOfficeBackend
{
    public class HQEmployee
    {
        private string _name;
        private string _email;
        private bool _isManager;
        private int _id;

        public string Name { get => _name; set => _name = value; }     
        public string Email { get => _email; set => _email = value; }    
        public bool IsManager { get => _isManager; set => _isManager = value; }      
        [JsonProperty("employee_id")]
        public int ID { get => _id; set => _id = value; }    
        [JsonProperty("date_of_birth")]
        public string DateOfBirth { get; set; }    
        [JsonProperty("branch_office_id")]
        public int BranchOfficeID { get; set; }     
        public float Pay { get; set; }      
        
    }
}