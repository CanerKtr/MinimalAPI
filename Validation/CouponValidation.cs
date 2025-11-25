using FluentValidation;
using MinimalAPI.Dto;

namespace MinimalAPI.Validation
{
    public class CouponValidation
    {
        public class CouponCreateValidation : AbstractValidator<CouponDtoWithoutId>
        {
            public CouponCreateValidation()
            {
                RuleFor(c => c.Name)
                    .NotNull().WithMessage("Name cannot be null.")
                    .NotEmpty().WithMessage("Name is required.")
                    .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

                RuleFor(c => c.Percent)
                    .InclusiveBetween(0, 100).WithMessage("Percent must be between 0 and 100.");

                RuleFor(c => c.IsActive)
                    .NotNull().WithMessage("IsActive is required.");
            }
        }
        public class CouponUpdateValidation : AbstractValidator<CouponDto>
        {
            public CouponUpdateValidation()
            {
                RuleFor(c => c.Id)
                    .GreaterThan(0).WithMessage("Id must be greater than 0.");

                RuleFor(c => c.Name)
                    .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.").When(c => c.Name != null)
                     // Name null değilse, yani gönderildiyse
                    .NotEmpty().WithMessage("Name cannot be empty when provided.").When(c => c.Name != null);

                RuleFor(c => c.Percent)
                    .InclusiveBetween(0, 100).WithMessage("Percent must be between 0 and 100.")
                    .When(c => c.Percent.HasValue);


            }
        }
    }
}
