### Description

A console program that can be downloaded through segmentation technology to maximize the utilization of your network bandwidth.

﻿### Arguments

| Name | Arg next           | Arg next type | What it can do?                                              |
| :--- | :----------------- | :-----------: | :----------------------------------------------------------- |
| -v   |                    |               | Display software version information.                        |
| -n   | [num, default 4]   |    Integer    | The number of shards for downloading resources in segments.  |
| -o   | [path, default ./] |    String     | Save resources to the specified path.                        |
| -a   |                    |               | Pop up a prompt window after downloading is completed.       |
| -s   |                    |               | Quiet mode, no output of any content, including download progress. |
| -l   | [url1 url2 ...]    |   String[]    | URL of the resource to download. Separate different urls with spaces. |
| -c   |                    |               | Attempt to open this file after downloading is complete.     |
| -h   |                    |               | Print help information.                                      |
| -m   | [speed]            |    String     | The speed limit field, for example, can be written as follows: 300kb/s, 500mb/s, only supports these three units of speed limit: kilobytes and megabytes. |

