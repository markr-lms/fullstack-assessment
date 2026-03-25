import axios from "axios";
import type { PaginatedList } from "~/types/common-types";
import type { LawFirm } from "~/types/law-firm-types";

export type IApi = ReturnType<typeof useApi>;

export default function useApi() {
  const apiBaseAddress = "";

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
  };
}
