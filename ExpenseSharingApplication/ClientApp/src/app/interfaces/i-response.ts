export interface IResponse<T> {

        data?: T;
        message?: string;
        isSuccess: boolean; 
}
