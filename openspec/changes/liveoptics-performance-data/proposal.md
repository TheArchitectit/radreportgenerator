# Proposal: Full Live Optics performance series ingestion

## Intent
Model supports IoHistory/CpuHistory and peak metrics; parser never fills them. Enable real analysis and charts.

## Scope
In: Sheet discovery for Live Optics performance tabs, peak metric calculation, model population.
Out: Real-time streaming.

## Approach
Best-effort schema detection across Live Optics export versions; document supported layouts; fallback empty history.
