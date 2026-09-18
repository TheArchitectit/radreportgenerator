# Domain Models Specification

## Purpose
In-memory model for a Live Optics assessment project.

## Requirements

### Requirement: ProjectInfo
ProjectInfo holds ProjectName, CreatedDate, ProjectId, and a list of ServerNode. Defaults: empty name, empty server list.

#### Scenario: Defaults
- GIVEN a new ProjectInfo
- THEN Servers is empty and ProjectName is empty string

### Requirement: ServerNode
ServerNode holds ServerName, OS, CPUCount, MemoryGB, Disks, and Performance profile.

#### Scenario: Defaults
- GIVEN a new ServerNode
- THEN Disks and Performance collections are non-null and empty

### Requirement: DiskDrive
DiskDrive holds DiskName, CapacityGB, FreeSpaceGB.

### Requirement: PerformanceProfile
PerformanceProfile holds PeakIOPS, PeakThroughputMBps, AvgLatencyMs, IoHistory, CpuHistory lists of MetricPoint.

### Requirement: MetricPoint
MetricPoint holds Timestamp and Value (may be zero or negative).
