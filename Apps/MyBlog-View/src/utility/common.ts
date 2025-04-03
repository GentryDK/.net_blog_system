export const transDate = (date:string)=>{
    let dateOj = new Date(date);
    var day = `${dateOj.getFullYear()}-${dateOj.getMonth()+1}-${dateOj.getDate()}`;
    return day;
}

export const getUpdatedCoverUrl = (url: string) => {
    return `${url}?t=${new Date().getTime()}`;
};