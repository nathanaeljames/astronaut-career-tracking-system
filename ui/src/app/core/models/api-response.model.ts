export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  responseCode?: number;
  data?: T;
}

export interface CreatePersonResponse extends ApiResponse {
  id?: number;
}

export interface GetPeopleResponse extends ApiResponse {
  people: Person[];
}

export interface Person {
  personId: number;
  name: string;
}
