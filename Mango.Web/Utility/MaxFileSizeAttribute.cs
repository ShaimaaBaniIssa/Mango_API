﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;
        public MaxFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > (_maxSize * 1024 * 1024))
                    return new ValidationResult($"Maximum allowed file size is {_maxSize} MB.");

            }
            return ValidationResult.Success;
        }
    }
}
// then consume it in productdto
// add model state in product controller
