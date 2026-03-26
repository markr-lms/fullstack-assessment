export type LawFirm = {
  id: string;
  name: string;
  address: string;
  phoneNumber: string;
  email: string;
  createdBy: string;
  createdAt: string;
};

export type CreateLawFirmRequest = Omit<
  LawFirm,
  "id" | "createdBy" | "createdAt"
>;

export type UpdateLawFirmRequest = Omit<LawFirm, "createdBy" | "createdAt">;
