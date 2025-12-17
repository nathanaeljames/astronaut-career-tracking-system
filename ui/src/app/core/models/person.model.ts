export interface Person {
  id: number;
  name: string;
}

export interface PersonWithDetails extends Person {
  astronautDetail?: AstronautDetail;
}

export interface AstronautDetail {
  id: number;
  personId: number;
  currentRank: string;
  currentDutyTitle: string;
  careerStartDate: string;
  careerEndDate?: string;
}
