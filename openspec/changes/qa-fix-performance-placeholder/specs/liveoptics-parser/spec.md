# Delta for LiveOptics Parser

## MODIFIED Requirements

### Requirement: Performance data (current stub)
Performance history fields exist on the model but the parser does NOT populate them today unless performance sheets are implemented. Unimplemented performance parsing MUST NOT silently pretend success without observable behavior (log or explicit empty result contract).

#### Scenario: Performance columns present
- GIVEN any valid Live Optics xlsx
- WHEN ParseFile completes
- THEN Performance.IoHistory and CpuHistory remain empty until performance parsing is implemented

#### Scenario: Stub invoked
- GIVEN performance parse not implemented
- WHEN ParseFile runs
- THEN history remains empty and logs state why
