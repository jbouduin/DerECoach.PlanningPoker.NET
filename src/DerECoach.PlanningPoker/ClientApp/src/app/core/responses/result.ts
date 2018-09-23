export class BaseResult {
  message: string;
  httpStatus: string;
  succeeded: boolean;
}

export class Result<T> extends BaseResult {
  value: T;
}
