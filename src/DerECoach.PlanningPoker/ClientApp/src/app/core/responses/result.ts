export class BaseResult {
  message: string;  
  succeeded: boolean;
}

export class Result<T> extends BaseResult {
  value: T;
}
