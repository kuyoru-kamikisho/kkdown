### Execute demonstration

![img](../ctest/show.gif)

### Description

A console program that can be downloaded through segmentation technology to maximize the utilization of your network bandwidth.

### Arguments

| Name | Arg next           | Arg next type | What it can do?                                                       |
|:-----|:-------------------|:-------------:|:----------------------------------------------------------------------|
| -v   |                    |               | Display software version information.                                 |
| -n   | [num, default 4]   |    Integer    | The number of shards for downloading resources in segments.           |
| -o   | [path, default ./] |    String     | Save resources to the specified path.                                 |
| -l   | [url1 url2 ...]    |   String[]    | URL of the resource to download. Separate different urls with spaces. |
| -h   |                    |               | Print help information.                                               |

### Note

Due to differences in interaction between Powershell and CMD, 
this application has not been implemented in Powershell yet. 
You should use CMD to execute this program.