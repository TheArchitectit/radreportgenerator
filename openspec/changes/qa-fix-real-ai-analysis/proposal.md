# Proposal: Replace mock research agent with pluggable analysis (QA-10)

## Intent
UI and README present "AI Research Agent" but service returns canned strings. Either implement a real provider or clearly mark mock and add IAnalysisService abstraction for later Copilot/OpenAI.

## Scope
In: IResearchAgent/IAnalysisService abstraction, mock implementation labeled Demo, optional settings for API key later.
Out: billing, production prompt hardening (later change).

## Approach
Phase A: interface + mock labeled DemoInsight; UI badge "Demo".
Phase B (later): HTTP client for real LLM when key configured.
