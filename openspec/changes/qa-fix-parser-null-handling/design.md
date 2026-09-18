# Design: Name default contract

Decision: use `string.Empty` for missing/blank Server Name; UI can display placeholder without mutating data.
Update parser accordingly; keep tests green; adjust test only if product chooses Unknown.
