param(
    [Parameter(Mandatory = $true)]
    [string]$ValidatorName,

    [switch]$Force
)

# If user writes "EventTypeDto" instead of "EventTypeDtoValidator"
if ($ValidatorName -notlike "*Validator") {
    $ValidatorName = "${ValidatorName}Validator"
}

$DtoName = $ValidatorName -replace "Validator$", ""
$TestClassName = "${ValidatorName}Tests"

$FolderPath = ".\Umbraco.Extension.Test\Validators"
$FilePath = Join-Path $FolderPath "$TestClassName.cs"

New-Item -ItemType Directory -Force -Path $FolderPath | Out-Null

if ((Test-Path $FilePath) -and (-not $Force)) {
    Write-Host "File already exists: $FilePath"
    Write-Host "Use -Force to overwrite."
    exit
}

$template = @'
using FluentValidation.TestHelper;
using Umbraco.Extension.Dtos;
using Umbraco.Extension.Validators;

namespace Umbraco.Extension.Test.Validators;

public class {{TestClassName}}
{
    private {{ValidatorName}} _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new {{ValidatorName}}();
    }

    [Test]
    public void Should_not_have_validation_errors_when_dto_is_valid()
    {
        // Arrange
        var dto = new {{DtoName}}
        {
            // TODO: Add valid test data
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
'@

$content = $template `
    -replace "{{TestClassName}}", $TestClassName `
    -replace "{{ValidatorName}}", $ValidatorName `
    -replace "{{DtoName}}", $DtoName

Set-Content -Path $FilePath -Value $content -Encoding UTF8

Write-Host "Created: $FilePath"