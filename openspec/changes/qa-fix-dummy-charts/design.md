# Design: Real chart data

Parser: scan sheets for timestamped IOPS/throughput columns; fill PerformanceProfile.
ViewModel: if no history, set series empty and status "No performance series in export".
Do not invent numbers in production path; keep dummy data only in unit-test fixtures if needed.
