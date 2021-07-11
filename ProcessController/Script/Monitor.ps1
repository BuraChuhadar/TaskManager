$RAM= Get-WMIObject Win32_PhysicalMemory | Measure -Property capacity -Sum | %{$_.sum/1Mb}
$cores = (Get-WmiObject Win32_Processor).NumberOfLogicalProcessors
$tmp = Get-WmiObject Win32_PerfFormattedData_PerfProc_Process | 
select-object -Property Name, @{Name = "CPU"; Expression = {($_.PercentProcessorTime)}}, @{Name = "PID"; Expression = {$_.IDProcess}}, @{"Name" = "Memory(MB)"; Expression = {[int]($_.WorkingSetPrivate/1mb)}}, @{"Name" = "Memory(%)"; Expression = {([math]::Round(($_.WorkingSetPrivate/1Mb)/$RAM*100,2))}}, @{Name="Disk(MB)"; Expression = {[Math]::Round(($_.IODataOperationsPersec / 1mb),2)}}, @{"Name"="Network"; Expression = { $_.IOReadBytesPersec }} |
Where-Object {$_.Name -notmatch "^(idle|_total|system)$"} |
Sort-Object -Property CPU -Descending
$tmp 
Write-Output $tmp