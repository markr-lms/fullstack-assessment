export type LawFirm = {
  id: string;
  name: string;
  address: string;
  phoneNumber: string;
  email: string;
  createdBy: string;
};

export type CreateLawFirmRequest = Omit<LawFirm, "id" | "createdBy">;

export type UpdateLawFirmRequest = Omit<LawFirm, "createdBy">;
