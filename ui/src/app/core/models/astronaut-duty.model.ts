export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
  dutyEndDate?: string;
}

export interface CreateAstronautDutyRequest {
  name: string;
  rank: string;
  dutyTitle: string;
  dutyStartDate: string;
}

export interface AstronautDutyHistory {
  person: Person;
  astronautDuties: AstronautDuty[];
}

export interface Person {
  id: number;
  name: string;
}
