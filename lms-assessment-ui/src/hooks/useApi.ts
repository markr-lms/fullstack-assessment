import axios from "axios";
import type { PaginatedList } from "~/types/common-types";
import type { CreateLawFirmRequest, LawFirm } from "~/types/law-firm-types";

export type IApi = ReturnType<typeof useApi>;

export default function useApi() {
  const apiBaseAddress = "https://localhost:7113";

  const config = {
    headers: {
      "X-User-Id": "aaaaaaaa-0000-0000-0000-000000000001",
    },
  };

  return {
    getLawFirms: async (pageNumber: number, pageSize: number) => {
      const url =
        `${apiBaseAddress}/lawfirms` +
        `?pageNumber=${pageNumber}` +
        `&pageSize=${pageSize}`;

      const { data } = await axios.get<PaginatedList<LawFirm>>(url, config);
      return data;
    },

    createLawFirm: async (request: CreateLawFirmRequest) => {
      const { data } = await axios.post<LawFirm>(
        `${apiBaseAddress}/lawfirms`,
        request,
        config,
      );
      return data;
    },
  };
}
