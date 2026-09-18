# Proposal: Core abstractions and DI container

## Intent
Planning docs require IDataParser/IReportGenerator/IChartProvider/IAnalysisService/IDebugService and Microsoft.Extensions.DependencyInjection. None exist.

## Scope
In: Interfaces in Core, ServiceConfiguration, factory types, unit tests for registrations.
Out: Concrete implementations beyond what's needed to compile.

## Approach
Define contracts first; adapt existing services to implement them; register in DI; ViewModel uses injected interfaces.
