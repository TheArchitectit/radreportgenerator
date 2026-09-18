# Research Agent Specification

## Purpose
Provide infrastructure analysis insights shown in the WPF AI sidebar.

## Requirements

### Requirement: Performance analysis (mock)
The agent SHALL return a non-null analysis string that includes the caller-supplied query text. Current implementation is simulated (delay + canned copy), not an LLM call.

#### Scenario: Query analysis
- GIVEN a query string
- WHEN AnalyzePerformanceAsync is called
- THEN the result is non-null and contains the query text

### Requirement: Hardware research (mock)
The agent SHALL return a non-null research string that includes the hardware model argument.

#### Scenario: Hardware lookup
- GIVEN a model name such as Dell R740
- WHEN ResearchHardwareAsync is called
- THEN the result is non-null and contains the model name

### Requirement: Not production AI
There is no network call, API key, or model provider integration today. Specs that claim "AI-powered" insights are aspirational until a real provider change is archived.
