// year: 'numeric'：将年份显示为完整的数字（例如，2024）。

// month: '2-digit'：将月份显示为两位数字（例如，09 表示九月）。

// day: '2-digit'：将日期显示为两位数字（例如，19）。

// hour: '2-digit'：将小时显示为两位数字（例如，23）。

// minute: '2-digit'：将分钟显示为两位数字（例如，59）。

// second: '2-digit'：将秒钟显示为两位数字（例如，09）。

export const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
    });
}  